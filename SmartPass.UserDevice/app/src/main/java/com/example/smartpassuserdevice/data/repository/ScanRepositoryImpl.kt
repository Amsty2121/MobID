// src/main/java/com/example/smartpassuserdevice/data/repository/ScanRepositoryImpl.kt
package com.example.smartpassuserdevice.data.repository

import com.example.smartpassuserdevice.data.model.ScanCreateReq
import com.example.smartpassuserdevice.data.model.ScanDto
import com.example.smartpassuserdevice.data.network.ScanApi

class ScanRepositoryImpl(
    private val api: ScanApi
) {
    suspend fun getScans(): List<ScanDto> = try {
        api.getScans()
    } catch (_: Exception) {
        emptyList()
    }

    suspend fun createScan(qrRawValue: String): ScanDto =
        api.postScan(ScanCreateReq(qrRawValue))
}
