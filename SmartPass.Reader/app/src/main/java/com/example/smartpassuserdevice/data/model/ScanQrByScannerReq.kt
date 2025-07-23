package com.example.smartpassuserdevice.data.model

data class ScanQrByScannerReq(
    val payload: String,
    val organizationId: String,
    val accessId: String
)