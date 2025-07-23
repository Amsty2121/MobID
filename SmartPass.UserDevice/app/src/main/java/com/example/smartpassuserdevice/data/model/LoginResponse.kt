package com.example.smartpassuserdevice.data.model

import java.util.UUID

data class LoginResponse(
    val id: UUID,
    val username: String,
    val token: String,
    val refreshToken: String
)