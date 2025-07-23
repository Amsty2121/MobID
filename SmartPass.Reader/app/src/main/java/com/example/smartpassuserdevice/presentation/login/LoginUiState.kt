package com.example.smartpassuserdevice.presentation.login

import com.example.smartpassuserdevice.data.model.LoginResponse

data class LoginUiState(
    val email: String = "",
    val password: String = "",
    val isLoading: Boolean = false,
    val error: String? = null,
    val success: Boolean = false,
    val loginResponse: LoginResponse? = null
)