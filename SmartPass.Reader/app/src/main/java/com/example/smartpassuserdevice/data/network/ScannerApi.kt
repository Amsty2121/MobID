package com.example.smartpassuserdevice.data.network

import com.example.smartpassuserdevice.data.model.AccessDto
import com.example.smartpassuserdevice.data.model.OrganizationDto
import com.example.smartpassuserdevice.data.model.ScanQrByScannerReq
import com.example.smartpassuserdevice.data.model.ScanQrByScannerRsp
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

interface ScannerApi {
    @GET("/api/organization/for-user")
    suspend fun getMyOrgs(): List<OrganizationDto>    // deja ai

    @GET("/api/organization/{organizationId}/accesses/all")
    suspend fun getAllOrgAccesses(@Path("organizationId") orgId: String): List<AccessDto>

    @POST("/api/scan/by-scanner")
    suspend fun scanByScanner(
        @Body req: ScanQrByScannerReq
    ): ScanQrByScannerRsp
}
