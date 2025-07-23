package com.example.smartpassuserdevice.data.model

data class ScanDto(
    val id: String,
    val qrCodeId: String,
    val scannedByUserId: String,
    val scannedByUserName: String,
    val scannedForUserId: String,
    val scannedForUserName: String,
    val scannedAt: String
)