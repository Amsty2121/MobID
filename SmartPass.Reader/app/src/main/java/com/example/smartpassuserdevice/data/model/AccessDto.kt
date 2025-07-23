package com.example.smartpassuserdevice.data.model

import java.util.UUID

data class AccessDto(
    val id: String,
    val name: String,
    val accessTypeName: String,
    val organizationName: String,
    val expirationDateTime: String?, // ISO date
    val isMultiScan: Boolean,
    val restrictToOrgMembers: Boolean,
    val restrictToOrgSharing: Boolean,
    val isActive: Boolean
)