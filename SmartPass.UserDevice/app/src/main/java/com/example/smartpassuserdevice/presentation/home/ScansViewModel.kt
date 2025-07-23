// src/main/java/com/example/smartpassuserdevice/presentation/home/ScansViewModel.kt
package com.example.smartpassuserdevice.presentation.home

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.smartpassuserdevice.data.model.ScanDto
import com.example.smartpassuserdevice.data.repository.ScanRepositoryImpl
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

class ScansViewModel(
    private val repo: ScanRepositoryImpl
) : ViewModel() {
    private val _scans = MutableStateFlow<List<ScanDto>>(emptyList())
    val scans: StateFlow<List<ScanDto>> = _scans.asStateFlow()

    init {
        viewModelScope.launch {
            _scans.value = repo.getScans()
        }
    }
}
