// --- Тосты (лёгкая замена alert()) ---
function ensureToastStack() {
    let stack = document.querySelector(".toast-stack");
    if (!stack) {
        stack = document.createElement("div");
        stack.className = "toast-stack";
        document.body.appendChild(stack);
    }
    return stack;
}

function showToast(message, type = "blue", duration = 4200) {
    const stack = ensureToastStack();
    const toast = document.createElement("div");
    toast.className = `toast toast-${type}`;
    toast.textContent = message;
    stack.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = "0";
        toast.style.transition = "opacity 0.3s ease";
        setTimeout(() => toast.remove(), 300);
    }, duration);
}
window.showToast = showToast;

// --- Игровой интерактив на главной странице (превью в hero) ---
document.addEventListener("DOMContentLoaded", () => {
    const mallet = document.getElementById("interactive-mallet");
    const puck = document.getElementById("interactive-puck");
    const arena = document.querySelector(".neon-arena-preview");

    if (mallet && puck && arena) {
        arena.addEventListener("mousemove", (e) => {
            const rect = arena.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const limitX = Math.max(20, Math.min(x, rect.width / 2 - 20));
            const limitY = Math.max(20, Math.min(y, rect.height - 20));

            mallet.style.left = `${limitX - 16}px`;
            mallet.style.top = `${limitY - 16}px`;

            const malletCenterX = limitX;
            const malletCenterY = limitY;
            const puckCenterX = puck.offsetLeft + 10;
            const puckCenterY = puck.offsetTop + 10;

            const dx = puckCenterX - malletCenterX;
            const dy = puckCenterY - malletCenterY;
            const distance = Math.sqrt(dx * dx + dy * dy);

            if (distance < 26) {
                const angle = Math.atan2(dy, dx);
                const pushForce = 40;
                let newPuckX = malletCenterX + Math.cos(angle) * (26 + pushForce) - 10;
                let newPuckY = malletCenterY + Math.sin(angle) * (26 + pushForce) - 10;

                newPuckX = Math.max(10, Math.min(newPuckX, rect.width - 30));
                newPuckY = Math.max(10, Math.min(newPuckY, rect.height - 30));

                puck.style.left = `${newPuckX}px`;
                puck.style.top = `${newPuckY}px`;
            }
        });

        arena.addEventListener("mouseleave", () => {
            puck.style.left = "48%";
            puck.style.top = "45%";
            mallet.style.left = "20%";
            mallet.style.top = "40%";
        });
    }

    // Живой счётчик XP/уровня-демо на главной (чисто декоративный, без ложных данных)
    const xpFill = document.getElementById("xp-demo-fill");
    if (xpFill) {
        requestAnimationFrame(() => {
            xpFill.style.width = xpFill.dataset.progress || "0%";
        });
    }
});

// --- Симуляция скачивания ---
function triggerDownload(platformName) {
    if (platformName === "PC Client") {
        showToast("Клиент для ПК пока в разработке. Играть на компьютере уже можно через веб-версию на itch.io.", "gold");
    } else if (platformName === "Android APK") {
        showToast("Открываю страницу релизов на GitHub…", "blue");
        window.open("https://github.com/ZebrarsGames/AirClash/releases", "_blank");
    }
}

// --- Отправка формы обратной связи AirClash ---
async function handleContactSubmit(event) {
    event.preventDefault();

    const form = event.target;
    const button = form.querySelector('button[type="submit"]');

    const originalButtonText = button.textContent;
    button.textContent = "Отправка сигнала...";
    button.disabled = true;

    const usernameInput = form.querySelector('input[type="text"]');
    const emailInput = form.querySelector('input[type="email"]');
    const messageInput = form.querySelector("textarea");

    const payload = {
        access_key: "fcbe9b27-2403-431d-a8b0-ef4804fcf167",
        username: usernameInput ? usernameInput.value : "",
        email: emailInput ? emailInput.value : "",
        message: messageInput ? messageInput.value : "",
    };

    try {
        const response = await fetch("https://api.web3forms.com/submit", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Accept: "application/json",
            },
            body: JSON.stringify(payload),
        });

        const result = await response.json();

        if (response.ok && result.success) {
            showToast("Сообщение успешно отправлено в центр управления AirClash! Мы ответим вам в ближайшее время.", "blue");
            form.reset();
        } else {
            showToast("Ошибка сервера: " + (result.message || "Неверный ключ"), "pink");
        }
    } catch (error) {
        console.error("Ошибка сети:", error);
        showToast("Не удалось отправить сигнал. Проверьте интернет-соединение.", "pink");
    } finally {
        button.textContent = originalButtonText;
        button.disabled = false;
    }
}
