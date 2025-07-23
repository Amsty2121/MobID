package com.example.smartpassuserdevice.data.model

data class RegisterRequest(
    val username: String,
    val email: String,
    val password: String
)