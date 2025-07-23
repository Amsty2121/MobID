package com.example.smartpassuserdevice.data.model

data class OrganizationDto(
    val id: String,
    val name: String,
    val description: String?,
    val ownerId: String,
    val ownerUsername: String,
    val createdAt: String, // ISO date string
    val isActive: Boolean
)