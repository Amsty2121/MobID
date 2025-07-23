package com.example.smartpassuserdevice.presentation.home


import com.example.smartpassuserdevice.data.model.AccessDto
import com.example.smartpassuserdevice.data.model.OrganizationDto
import com.example.smartpassuserdevice.data.model.ScanQrByScannerRsp

data class HomeUiState(
    val orgs: List<OrganizationDto> = emptyList(),
    val accesses: List<AccessDto>   = emptyList(),
    val selectedOrg: OrganizationDto? = null,
    val selectedAccess: AccessDto?    = null,
    val isLoadingOrgs: Boolean        = false,
    val isLoadingAccesses: Boolean    = false,
    val showScannerDialog: Boolean    = false,
    val scanResult: ScanQrByScannerRsp? = null,
    val error: String?                 = null
)