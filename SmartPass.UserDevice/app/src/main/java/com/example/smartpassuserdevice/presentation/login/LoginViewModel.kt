package com.example.smartpassuserdevice.presentation.login

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.smartpassuserdevice.data.repository.AuthRepositoryImpl
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

class LoginViewModel(
    private val repo: AuthRepositoryImpl
) : ViewModel() {

    private val _state = MutableStateFlow(LoginUiState())
    val uiState: StateFlow<LoginUiState> = _state.asStateFlow()

    fun onEmailChange(v: String) = _state.update { it.copy(email = v) }
    fun onPasswordChange(v: String) = _state.update { it.copy(password = v) }

    fun login() {
        viewModelScope.launch {
            _state.update { it.copy(isLoading = true, error = null) }
            repo.login(_state.value.email, _state.value.password)
                .onSuccess { resp ->
                    _state.update { it.copy(isLoading = false, success = true, loginResponse = resp) }
                }
                .onFailure { ex ->
                    _state.update { it.copy(isLoading = false, error = ex.message) }
                }
        }
    }
}