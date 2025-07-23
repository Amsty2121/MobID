package com.example.smartpassuserdevice.util

import android.content.Context
import android.content.SharedPreferences
import com.example.smartpassuserdevice.data.model.LoginResponse

object CacheUtils {
    private const val PREF_NAME    = "auth_data"
    private const val KEY_TOKEN    = "token"
    private const val KEY_REFRESH  = "refresh_token"
    private const val KEY_USER_ID  = "user_id"
    private const val KEY_USERNAME = "username"

    private fun prefs(ctx: Context): SharedPreferences =
        ctx.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE)

    fun saveLoginResponse(ctx: Context, resp: LoginResponse?) {
        if (resp == null) return
        prefs(ctx).edit().apply {
            putString(KEY_TOKEN, "Bearer ${resp.token}")
            putString(KEY_REFRESH, resp.refreshToken)
            putString(KEY_USER_ID, resp.id.toString())
            putString(KEY_USERNAME, resp.username)
            apply()
        }
    }

    fun getToken(ctx: Context): String? =
        prefs(ctx).getString(KEY_TOKEN, null)

    /** Returnează refresh-token-ul. */
    fun getRefreshToken(ctx: Context): String? =
        prefs(ctx).getString(KEY_REFRESH, null)

    /** Returnează userId-ul salvat (string UUID) sau null. */
    fun getUserId(ctx: Context): String? =
        ctx.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE)
            .getString(KEY_USER_ID, null)
    /** Returnează username-ul salvat. */
    fun getUsername(ctx: Context): String? =
        prefs(ctx).getString(KEY_USERNAME, null)

    fun clearAuthData(ctx: Context) {
        prefs(ctx).edit().apply {
            remove(KEY_TOKEN)
            remove(KEY_REFRESH)
            remove(KEY_USER_ID)
            remove(KEY_USERNAME)
            apply()
        }
    }
}
