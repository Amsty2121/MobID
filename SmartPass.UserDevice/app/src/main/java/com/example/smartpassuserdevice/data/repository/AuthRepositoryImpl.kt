package com.example.smartpassuserdevice.data.repository

import android.content.Context
import com.example.smartpassuserdevice.data.model.LoginRequest
import com.example.smartpassuserdevice.data.model.LoginResponse
import com.example.smartpassuserdevice.data.model.RegisterRequest
import com.example.smartpassuserdevice.data.network.AuthApi
import com.example.smartpassuserdevice.util.CacheUtils

class AuthRepositoryImpl(
    private val api: AuthApi,
    private val ctx: Context
) {
    suspend fun login(email: String, password: String) =
        try {
            val resp = api.login(LoginRequest(email, password))
            CacheUtils.saveLoginResponse(ctx, resp)
            Result.success(resp)
        } catch (e: Exception) {
            Result.failure<LoginResponse>(e)
        }

    suspend fun register(username: String, email: String, password: String) =
        try {
            api.register(RegisterRequest(username, email, password))
            Result.success(Unit)
        } catch (e: Exception) {
            Result.failure<Unit>(e)
        }
}
