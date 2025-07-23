package com.example.smartpassuserdevice.data.repository

import com.example.smartpassuserdevice.data.model.AccessDto
import com.example.smartpassuserdevice.data.model.OrganizationDto
import com.example.smartpassuserdevice.data.model.ScanQrByScannerReq
import com.example.smartpassuserdevice.data.model.ScanQrByScannerRsp
import com.example.smartpassuserdevice.data.network.ScannerApi

class ScannerRepositoryImpl(
    private val api: ScannerApi
) {

    suspend fun getMyOrganizations(): Result<List<OrganizationDto>> =
        try {
            val orgs = api.getMyOrgs()
            Result.success(orgs)
        } catch (e: Exception) {
            Result.failure(e)
        }

    suspend fun getAllOrganizationAccesses(organizationId: String): Result<List<AccessDto>> =
        try {
            val accesses = api.getAllOrgAccesses(organizationId)
            Result.success(accesses)
        } catch (e: Exception) {
            Result.failure(e)
        }

    suspend fun scanUserQr(
        payload: String,
        organizationId: String,
        accessId: String
    ): Result<ScanQrByScannerRsp> =
        try {
            val req = ScanQrByScannerReq(
                payload = payload,
                organizationId = organizationId,
                accessId = accessId
            )
            val resp = api.scanByScanner(req)
            Result.success(resp)
        } catch (e: Exception) {
            Result.failure(e)
        }
}
