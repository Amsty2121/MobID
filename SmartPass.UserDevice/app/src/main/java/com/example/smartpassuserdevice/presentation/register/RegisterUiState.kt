package com.example.smartpassuserdevice.presentation.register

data class RegisterUiState(
    val username: String = "",
    val email:     String = "",
    val password:  String = "",
    val isLoading: Boolean = false,
    val error:     String? = null,
    val success:   Boolean = false
)