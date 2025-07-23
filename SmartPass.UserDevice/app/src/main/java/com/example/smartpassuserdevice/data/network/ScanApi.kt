// src/main/java/com/example/smartpassuserdevice/data/network/ScanApi.kt
package com.example.smartpassuserdevice.data.network

import com.example.smartpassuserdevice.data.model.ScanDto
import com.example.smartpassuserdevice.data.model.ScanCreateReq
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

interface ScanApi {
    @GET("scan/user")
    suspend fun getScans(): List<ScanDto>

    @POST("scan")
    suspend fun postScan(@Body request: ScanCreateReq): ScanDto
}
