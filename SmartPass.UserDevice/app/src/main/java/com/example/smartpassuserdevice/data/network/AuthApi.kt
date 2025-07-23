package com.example.smartpassuserdevice.data.network

import com.example.smartpassuserdevice.data.model.LoginRequest
import com.example.smartpassuserdevice.data.model.LoginResponse
import com.example.smartpassuserdevice.data.model.RegisterRequest
import com.example.smartpassuserdevice.data.model.RegisterResponse
import retrofit2.http.Body
import retrofit2.http.POST

interface AuthApi {
    @POST("auth/login")
    suspend fun login(@Body req: LoginRequest): LoginResponse

    @POST("auth/register")
    suspend fun register(@Body req: RegisterRequest): RegisterResponse
}
