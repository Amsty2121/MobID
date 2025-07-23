package com.example.smartpassuserdevice.presentation.register

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.smartpassuserdevice.data.repository.AuthRepositoryImpl
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

class RegisterViewModel(
    private val repo: AuthRepositoryImpl
) : ViewModel() {

    private val _state = MutableStateFlow(RegisterUiState())
    val uiState: StateFlow<RegisterUiState> = _state.asStateFlow()

    fun onUsernameChange(v: String) = _state.update { it.copy(username = v) }
    fun onEmailChange(v: String) = _state.update { it.copy(email = v) }
    fun onPasswordChange(v: String) = _state.update { it.copy(password = v) }

    fun register() {
        viewModelScope.launch {
            _state.update { it.copy(isLoading = true, error = null) }
            repo.register(_state.value.username, _state.value.email, _state.value.password)
                .onSuccess {
                    _state.update { it.copy(isLoading = false, success = true) }
                }
                .onFailure { ex ->
                    _state.update { it.copy(isLoading = false, error = ex.message) }
                }
        }
    }
}