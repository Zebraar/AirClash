package com.epictransport.helper;

import android.content.Context;
import androidx.annotation.Keep;

@Keep
public class EOSInitializer {
    static {
        try {
            System.loadLibrary("EOSSDK");
            android.util.Log.d("EOSInitializer", "EOSSDK native library preloaded successfully!");
        } catch (Throwable t) {
            android.util.Log.e("EOSInitializer", "Failed to preload EOSSDK native library", t);
        }
    }

    public static void init() {

    }
}