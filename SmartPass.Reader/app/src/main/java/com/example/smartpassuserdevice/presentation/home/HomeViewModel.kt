package com.example.smartpassuserdevice.presentation.home

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.smartpassuserdevice.data.model.AccessDto
import com.example.smartpassuserdevice.data.model.OrganizationDto
import com.example.smartpassuserdevice.data.repository.ScannerRepositoryImpl
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

class HomeViewModel(
    private val repo: ScannerRepositoryImpl
) : ViewModel() {

    private val _uiState = MutableStateFlow(HomeUiState())
    val uiState: StateFlow<HomeUiState> = _uiState.asStateFlow()

    fun loadOrgs() = viewModelScope.launch {
        _uiState.update { it.copy(isLoadingOrgs = true, error = null) }
        repo.getMyOrganizations()
            .onSuccess { orgs ->
                _uiState.update { it.copy(orgs = orgs, isLoadingOrgs = false) }
            }
            .onFailure { ex ->
                _uiState.update { it.copy(isLoadingOrgs = false, error = ex.message) }
            }
    }

    fun onOrgSelected(org: OrganizationDto) {
        _uiState.update {
            it.copy(selectedOrg = org, isLoadingAccesses = true, error = null)
        }
        viewModelScope.launch {
            repo.getAllOrganizationAccesses(org.id)
                .onSuccess { accesses ->
                    _uiState.update {
                        it.copy(accesses = accesses, isLoadingAccesses = false)
                    }
                }
                .onFailure { ex ->
                    _uiState.update {
                        it.copy(isLoadingAccesses = false, error = ex.message)
                    }
                }
        }
    }

    fun onAccessSelected(access: AccessDto) {
        _uiState.update { it.copy(selectedAccess = access) }
    }

    fun startScan() {
        _uiState.update { it.copy(showScannerDialog = true) }
    }

    fun handleScanned(payload: String) {
        _uiState.update { it.copy(showScannerDialog = false, scanResult = null, error = null) }
        val orgId    = _uiState.value.selectedOrg?.id ?: return
        val accessId = _uiState.value.selectedAccess?.id ?: return

        viewModelScope.launch {
            repo.scanUserQr(payload, orgId, accessId)
                .onSuccess { resp ->
                    _uiState.update { it.copy(scanResult = resp) }
                }
                .onFailure { ex ->
                    _uiState.update { it.copy(error = ex.message) }
                }
            delay(1_000)
            _uiState.update { it.copy(scanResult = null) }
        }
    }
}
