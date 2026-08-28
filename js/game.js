// Мини-версия AirClash: играбельный аэрохоккей против ИИ прямо в браузере.
// Демонстрирует систему сложности ботов, описанную в самой игре.
(function () {
    const canvas = document.getElementById("game-canvas");
    if (!canvas) return;

    const ctx = canvas.getContext("2d");

    // Логические размеры поля (не зависят от CSS-масштаба канваса)
    const W = 700;
    const H = 400;
    canvas.width = W;
    canvas.height = H;

    const GOAL_HALF = 60;
    const MALLET_R = 22;
    const PUCK_R = 12;
    const WIN_SCORE = 7;

    // Параметры сложности — аналог PlayerPrefs "Difficulty" / "BotOffsetX" / "BotOffsetY"
    // из оригинального BotsAI.cs. moveSpeed — базовая скорость (px/кадр),
    // offsetX/offsetY — насколько неточно бот занимает защитную позицию,
    // puckKoof — угол атаки при подъезде к шайбе (аналог PuckKoof.y).
    const DIFFICULTIES = {
        easy: { label: "Лёгкий", moveSpeed: 3.4, offsetX: 48, offsetY: 60, puckKoof: 28 },
        medium: { label: "Средний", moveSpeed: 4.6, offsetX: 34, offsetY: 44, puckKoof: 20 },
        hard: { label: "Сложный", moveSpeed: 6.4, offsetX: 20, offsetY: 26, puckKoof: 11 },
        extreme: { label: "Экстрим", moveSpeed: 8.6, offsetX: 9, offsetY: 11, puckKoof: 4 },
    };

    let currentDifficulty = "medium";

    const BOT_START = { x: W * 0.82, y: H * 0.5 };
    const restartBtn = document.getElementById("restart-match");
    const timerOverlay = document.getElementById("timer-overlay");
    const timerText = document.getElementById("timer-overlay-text");

    const state = {
        player: { x: W * 0.18, y: H * 0.5, vx: 0, vy: 0 },
        bot: { x: W * 0.82, y: H * 0.5, vx: 0, vy: 0 },
        puck: { x: W * 0.5, y: H * 0.5, vx: 0, vy: 0 },
        scoreYou: 0,
        scoreBot: 0,
        running: true,
        pointerActive: false,
        gameOver: false,
        botSpeedFactor: 1, // аналог UpdateBotSpeed() — камбэк-механика по разнице счёта
        timerOn: false, // аналог TimerScr.TimerOn — блокирует ИИ и физику на время таймера
    };

    let countdownHandle = null;

    // --- Таймер гола / старта (порт TimerScr.cs) ---
    // Простые синтезированные звуки через Web Audio — заменяют
    // AudioClip[] goalSounds / AudioClip timerSound, для которых нет файлов.
    function playBeep(freq, duration, type, volume) {
        try {
            const AudioCtx = window.AudioContext || window.webkitAudioContext;
            if (!AudioCtx) return;
            if (!window.__airclashAudioCtx) window.__airclashAudioCtx = new AudioCtx();
            const audioCtx = window.__airclashAudioCtx;
            const osc = audioCtx.createOscillator();
            const gain = audioCtx.createGain();
            osc.type = type;
            osc.frequency.value = freq;
            gain.gain.value = volume;
            osc.connect(gain).connect(audioCtx.destination);
            osc.start();
            gain.gain.exponentialRampToValueAtTime(0.0001, audioCtx.currentTime + duration);
            osc.stop(audioCtx.currentTime + duration);
        } catch (e) {
            // Web Audio недоступен — просто без звука
        }
    }

    function showTimerOverlay(text) {
        if (!timerOverlay || !timerText) return;
        timerText.textContent = text;
        timerOverlay.classList.add("is-active");
    }

    function hideTimerOverlay() {
        if (!timerOverlay) return;
        timerOverlay.classList.remove("is-active");
    }

    function setRestartInteractable(interactable) {
        if (restartBtn) restartBtn.disabled = !interactable;
    }

    // Аналог TimerScr.Goal(): показываем "GOAL", блокируем рестарт и игру,
    // проигрываем гол-звук, через 1 секунду запускаем обратный отсчёт.
    function goalOverlay(direction) {
        clearTimeout(countdownHandle);
        clearInterval(countdownHandle);
        state.timerOn = true;
        setRestartInteractable(false);
        showTimerOverlay("GOAL");
        playBeep(200, 0.4, "sawtooth", 0.07);
        state.puck.x = W / 2;
        state.puck.y = H / 2;
        state.puck.vx = 0;
        state.puck.vy = 0;
        countdownHandle = setTimeout(() => timerStart(direction), 1000);
    }

    // Аналог TimerScr.TimerStart() + Timer(): показываем "4","3","2","1" с интервалом
    // в секунду, затем скрываем таймер, разблокируем рестарт и возобновляем игру.
    function timerStart(direction) {
        let timeLeft = 4;
        state.timerOn = true;
        setRestartInteractable(false);
        showTimerOverlay(String(timeLeft));
        playBeep(440, 0.12, "square", 0.05);

        countdownHandle = setInterval(() => {
            timeLeft -= 1;

            if (timeLeft <= 0) {
                clearInterval(countdownHandle);
                countdownHandle = null;
                hideTimerOverlay();
                state.timerOn = false;
                setRestartInteractable(true);
                resetPuck(direction || (Math.random() > 0.5 ? 1 : -1));
            } else {
                showTimerOverlay(String(timeLeft));
                playBeep(440, 0.12, "square", 0.05);
            }
        }, 1000);
    }

    function resetPuck(direction) {
        state.puck.x = W / 2;
        state.puck.y = H / 2;
        const angle = (Math.random() - 0.5) * 0.6;
        const speed = 3.2;
        state.puck.vx = Math.cos(angle) * speed * direction;
        state.puck.vy = Math.sin(angle) * speed;
    }

    function resetMatch() {
        clearTimeout(countdownHandle);
        clearInterval(countdownHandle);
        countdownHandle = null;
        state.scoreYou = 0;
        state.scoreBot = 0;
        state.gameOver = false;
        state.botSpeedFactor = 1;
        state.player.x = W * 0.18;
        state.player.y = H * 0.5;
        state.bot.x = W * 0.82;
        state.bot.y = H * 0.5;
        state.puck.x = W / 2;
        state.puck.y = H / 2;
        state.puck.vx = 0;
        state.puck.vy = 0;
        updateScoreboard();
        setStatus(`Матч начат. Уровень бота: <strong>${DIFFICULTIES[currentDifficulty].label}</strong>. Первый до ${WIN_SCORE}. Если начнёшь сильно вести — бот прибавит скорость.`);
        timerStart(Math.random() > 0.5 ? 1 : -1);
    }

    // Аналог UpdateBotSpeed(score1, score2) из оригинала: бот "злится" и ускоряется,
    // когда сильно проигрывает, и сбавляет обороты, когда сильно выигрывает.
    function updateBotSpeedFactor() {
        const diff = state.scoreBot - state.scoreYou;
        let factor = 1;

        if (diff >= 10) factor = 1 / 3;
        else if (diff >= 7) factor = 1 / 2.5;
        else if (diff >= 5) factor = 1 / 2;
        else if (diff >= 3) factor = 1 / 1.5;
        else if (diff <= -10) factor = 2;
        else if (diff <= -7) factor = 1.7;
        else if (diff <= -5) factor = 1.5;
        else if (diff <= -3) factor = 1.2;

        const prevFactor = state.botSpeedFactor;
        state.botSpeedFactor = factor;

        if (factor !== prevFactor && window.showToast) {
            if (factor > 1) {
                window.showToast("Бот отстаёт и включает камбэк-режим — стал быстрее!", "pink");
            } else if (factor < 1) {
                window.showToast("Бот сильно ведёт и сбавляет скорость, давая тебе шанс.", "gold");
            }
        }
    }

    function updateScoreboard() {
        const you = document.getElementById("score-you");
        const bot = document.getElementById("score-bot");
        if (you) you.textContent = state.scoreYou;
        if (bot) bot.textContent = state.scoreBot;
    }

    function setStatus(html) {
        const el = document.getElementById("match-status");
        if (el) el.innerHTML = html;
    }

    // --- Управление игроком (мышь / тач) ---
    function pointerToCanvas(clientX, clientY) {
        const rect = canvas.getBoundingClientRect();
        const scaleX = W / rect.width;
        const scaleY = H / rect.height;
        return {
            x: (clientX - rect.left) * scaleX,
            y: (clientY - rect.top) * scaleY,
        };
    }

    function movePlayerTo(x, y) {
        const clampedX = Math.max(MALLET_R, Math.min(x, W / 2 - MALLET_R));
        const clampedY = Math.max(MALLET_R, Math.min(y, H - MALLET_R));
        state.player.vx = clampedX - state.player.x;
        state.player.vy = clampedY - state.player.y;
        state.player.x = clampedX;
        state.player.y = clampedY;
    }

    canvas.addEventListener("mousemove", (e) => {
        const p = pointerToCanvas(e.clientX, e.clientY);
        movePlayerTo(p.x, p.y);
    });

    canvas.addEventListener(
        "touchmove",
        (e) => {
            e.preventDefault();
            const touch = e.touches[0];
            const p = pointerToCanvas(touch.clientX, touch.clientY);
            movePlayerTo(p.x, p.y);
        },
        { passive: false }
    );

    // --- ИИ бота (порт логики BotsAI.cs) ---
    // Масштаб: оригинальная арена Unity ~16 юнитов в ширину / ~9 в высоту,
    // наш канвас 700×400, поэтому 1 юнит ≈ 43.75px по X и ≈ 44.4px по Y.
    const KX = (W / 2) / 8;
    const KY = (H / 2) / 4.5;
    const ZONE_SHALLOW = 2 * KX; // puck.position.x > 0 && < 2
    const ZONE_FOUR = 4 * KX; // puck.position.x < -4
    const ZONE_SIX = 6 * KX; // puck.position.x < -6
    const Y_FOUR = 4 * KY; // puck.y > 4 || puck.y < -4
    const Y_THREE_FIVE = 3.5 * KY; // puck.y > 3.5 || puck.y < -3.5

    function moveTowards(obj, targetX, targetY, maxDelta) {
        const dx = targetX - obj.x;
        const dy = targetY - obj.y;
        const dist = Math.sqrt(dx * dx + dy * dy);
        if (dist <= maxDelta || dist === 0) {
            obj.x = targetX;
            obj.y = targetY;
        } else {
            obj.x += (dx / dist) * maxDelta;
            obj.y += (dy / dist) * maxDelta;
        }
    }

    function updateBot() {
        const cfg = DIFFICULTIES[currentDifficulty];
        const moveSpeed = cfg.moveSpeed * state.botSpeedFactor;

        // relX > 0 — шайба на нашей половине (у ворот бота), relX < 0 — у игрока.
        // Это зеркало условий оригинала, где x < 0 означало "своя половина".
        const relX = state.puck.x - W / 2;
        const dy = state.puck.y - H / 2;

        let targetX;
        let targetY;
        let speed;

        if (relX < 0 && relX > -ZONE_SHALLOW) {
            // a) шайба едва перешла к игроку — не бежим за ней, а перестраиваемся
            // в защитную позицию (зеркальное отражение + отступ назад + разброс по Y)
            const randomSign = Math.random() > 0.5 ? 1 : -1;
            targetX = W - state.puck.x + cfg.offsetX;
            targetY = state.puck.y + randomSign * cfg.offsetY;
            speed = moveSpeed / 4;
        } else if (relX <= -ZONE_SHALLOW) {
            // b) шайба глубоко на половине игрока — спешить некуда, едем в старт
            targetX = BOT_START.x;
            targetY = BOT_START.y;
            speed = moveSpeed / 3;
        } else if (relX > 0 && Math.abs(dy) > Y_FOUR) {
            // c) шайба у нас, но далеко от центра по вертикали — откатываемся в старт
            targetX = BOT_START.x;
            targetY = BOT_START.y;
            speed = moveSpeed / 4;
        } else if (relX > ZONE_SIX && Math.abs(dy) > Y_THREE_FIVE) {
            // d) то же самое у самых наших ворот
            targetX = BOT_START.x;
            targetY = BOT_START.y;
            speed = moveSpeed / 4;
        } else if (relX > ZONE_SIX) {
            // e) шайба почти у наших ворот — летим напрямую, без хитростей
            targetX = state.puck.x;
            targetY = state.puck.y;
            speed = moveSpeed * 1.5;
        } else if (relX > ZONE_FOUR) {
            // f) шайба приближается — подъезжаем под углом, а не в лоб,
            // чтобы отбить её к центру, а не просто остановить
            targetX = state.puck.x;
            targetY = state.puck.y - Math.sign(dy || 1) * cfg.puckKoof;
            speed = moveSpeed;
        } else {
            // g) шайба в средней зоне нашей половины — та же логика подъезда под углом
            targetX = state.puck.x;
            targetY = state.puck.y - Math.sign(dy || 1) * cfg.puckKoof;
            speed = moveSpeed;
        }

        targetX = Math.max(W / 2 + MALLET_R, Math.min(targetX, W - MALLET_R));
        targetY = Math.max(MALLET_R, Math.min(targetY, H - MALLET_R));

        const prevX = state.bot.x;
        const prevY = state.bot.y;

        moveTowards(state.bot, targetX, targetY, speed);

        state.bot.x = Math.max(W / 2 + MALLET_R, Math.min(state.bot.x, W - MALLET_R));
        state.bot.y = Math.max(MALLET_R, Math.min(state.bot.y, H - MALLET_R));

        state.bot.vx = state.bot.x - prevX;
        state.bot.vy = state.bot.y - prevY;
    }

    // --- Физика шайбы ---
    function resolveCollision(mallet) {
        const dx = state.puck.x - mallet.x;
        const dy = state.puck.y - mallet.y;
        const dist = Math.sqrt(dx * dx + dy * dy);
        const minDist = MALLET_R + PUCK_R;

        if (dist < minDist && dist > 0) {
            const nx = dx / dist;
            const ny = dy / dist;

            state.puck.x = mallet.x + nx * minDist;
            state.puck.y = mallet.y + ny * minDist;

            const relVx = state.puck.vx - mallet.vx;
            const relVy = state.puck.vy - mallet.vy;
            const dot = relVx * nx + relVy * ny;

            if (dot < 0) {
                state.puck.vx -= 2 * dot * nx;
                state.puck.vy -= 2 * dot * ny;
            }

            state.puck.vx += mallet.vx * 0.5;
            state.puck.vy += mallet.vy * 0.5;

            const speed = Math.sqrt(state.puck.vx ** 2 + state.puck.vy ** 2);
            const maxSpeed = 14;
            if (speed > maxSpeed) {
                state.puck.vx = (state.puck.vx / speed) * maxSpeed;
                state.puck.vy = (state.puck.vy / speed) * maxSpeed;
            }
        }
    }

    function step() {
        if (!state.running || state.gameOver || state.timerOn) return;

        updateBot();

        state.puck.x += state.puck.vx;
        state.puck.y += state.puck.vy;
        state.puck.vx *= 0.995;
        state.puck.vy *= 0.995;

        // Верх / низ
        if (state.puck.y - PUCK_R < 0) {
            state.puck.y = PUCK_R;
            state.puck.vy *= -1;
        } else if (state.puck.y + PUCK_R > H) {
            state.puck.y = H - PUCK_R;
            state.puck.vy *= -1;
        }

        const goalTop = H / 2 - GOAL_HALF;
        const goalBottom = H / 2 + GOAL_HALF;
        const inGoalMouth = state.puck.y > goalTop && state.puck.y < goalBottom;

        // Левая стена / ворота
        if (state.puck.x - PUCK_R < 0) {
            if (inGoalMouth) {
                state.scoreBot += 1;
                afterGoal();
                return;
            } else {
                state.puck.x = PUCK_R;
                state.puck.vx *= -1;
            }
        }

        // Правая стена / ворота
        if (state.puck.x + PUCK_R > W) {
            if (inGoalMouth) {
                state.scoreYou += 1;
                afterGoal(1);
                return;
            } else {
                state.puck.x = W - PUCK_R;
                state.puck.vx *= -1;
            }
        }

        resolveCollision(state.player);
        resolveCollision(state.bot);
    }

    function afterGoal(direction) {
        updateScoreboard();
        updateBotSpeedFactor();
        if (state.scoreYou >= WIN_SCORE || state.scoreBot >= WIN_SCORE) {
            state.gameOver = true;
            const youWin = state.scoreYou > state.scoreBot;
            setStatus(
                youWin
                    ? `🏆 Победа! Ты обыграл бота уровня <strong>${DIFFICULTIES[currentDifficulty].label}</strong> ${state.scoreYou}:${state.scoreBot}.`
                    : `Бот оказался сильнее — ${state.scoreBot}:${state.scoreYou}. Попробуй ещё раз или выбери уровень полегче.`
            );
            if (window.showToast) {
                window.showToast(
                    youWin ? "Матч выигран! Отличная игра." : "Матч проигран. Попробуй ещё раз!",
                    youWin ? "gold" : "pink"
                );
            }
            return;
        }
        goalOverlay(direction || -1);
    }

    function draw() {
        ctx.clearRect(0, 0, W, H);

        // Поле
        ctx.fillStyle = "rgba(10, 15, 35, 0.001)";
        ctx.fillRect(0, 0, W, H);

        // Центральная линия
        ctx.strokeStyle = "rgba(255, 0, 85, 0.5)";
        ctx.lineWidth = 2;
        ctx.setLineDash([10, 10]);
        ctx.beginPath();
        ctx.moveTo(W / 2, 0);
        ctx.lineTo(W / 2, H);
        ctx.stroke();
        ctx.setLineDash([]);

        // Центральный круг
        ctx.strokeStyle = "rgba(0, 240, 255, 0.35)";
        ctx.beginPath();
        ctx.arc(W / 2, H / 2, 45, 0, Math.PI * 2);
        ctx.stroke();

        // Ворота
        ctx.fillStyle = "#ff0055";
        ctx.shadowColor = "#ff0055";
        ctx.shadowBlur = 12;
        ctx.fillRect(0, H / 2 - GOAL_HALF, 6, GOAL_HALF * 2);
        ctx.fillRect(W - 6, H / 2 - GOAL_HALF, 6, GOAL_HALF * 2);
        ctx.shadowBlur = 0;

        // Биты
        drawCircle(state.player.x, state.player.y, MALLET_R, "#00f0ff");
        drawCircle(state.bot.x, state.bot.y, MALLET_R, "#ff0055");

        // Шайба
        drawCircle(state.puck.x, state.puck.y, PUCK_R, "#ffffff");
    }

    function drawCircle(x, y, r, color) {
        ctx.beginPath();
        ctx.arc(x, y, r, 0, Math.PI * 2);
        ctx.fillStyle = color;
        ctx.shadowColor = color;
        ctx.shadowBlur = 16;
        ctx.fill();
        ctx.strokeStyle = "#fff";
        ctx.lineWidth = 2;
        ctx.stroke();
        ctx.shadowBlur = 0;
    }

    function loop() {
        step();
        draw();
        requestAnimationFrame(loop);
    }

    // --- UI: выбор сложности ---
    document.querySelectorAll(".diff-btn").forEach((btn) => {
        btn.addEventListener("click", () => {
            document.querySelectorAll(".diff-btn").forEach((b) => b.classList.remove("active"));
            btn.classList.add("active");
            currentDifficulty = btn.dataset.difficulty;
            resetMatch();
        });
    });

    if (restartBtn) {
        restartBtn.addEventListener("click", resetMatch);
    }

    resetMatch();
    loop();
})();
