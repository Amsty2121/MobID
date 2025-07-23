package com.example.smartpassuserdevice.presentation.home

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.smartpassuserdevice.data.model.AccessDto
import com.example.smartpassuserdevice.data.repository.AccessRepositoryImpl
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

class AccessesViewModel(
    private val repo: AccessRepositoryImpl
) : ViewModel() {
    private val _accesses = MutableStateFlow<List<AccessDto>>(emptyList())
    val accesses: StateFlow<List<AccessDto>> = _accesses.asStateFlow()

    init {
        viewModelScope.launch {
            _accesses.value = repo.getAccesses()
        }
    }
}