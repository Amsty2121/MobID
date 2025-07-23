package com.example.smartpassuserdevice.data.model

data class RegisterResponse(
    val id: String,
    val email: String,
    val username: String,
    val roles: List<String>
)