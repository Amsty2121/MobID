package com.example.smartpassuserdevice.presentation.home

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.smartpassuserdevice.data.model.ScanDto
import com.example.smartpassuserdevice.data.repository.ScanRepositoryImpl
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

sealed class ScanUiState {
    object Idle    : ScanUiState()
    object Loading : ScanUiState()
    data class Success(val scan: ScanDto) : ScanUiState()
    data class Error  (val message: String) : ScanUiState()
}

class ScanViewModel(
    private val repo: ScanRepositoryImpl
) : ViewModel() {
    private val _uiState = MutableStateFlow<ScanUiState>(ScanUiState.Idle)
    val uiState: StateFlow<ScanUiState> = _uiState.asStateFlow()

    fun createScan(qrRawValue: String) {
        viewModelScope.launch {
            _uiState.value = ScanUiState.Loading
            try {
                val dto = repo.createScan(qrRawValue)
                _uiState.value = ScanUiState.Success(dto)
            } catch (ex: Exception) {
                _uiState.value = ScanUiState.Error(ex.message ?: "Unknown error")
            }
        }
    }
}
