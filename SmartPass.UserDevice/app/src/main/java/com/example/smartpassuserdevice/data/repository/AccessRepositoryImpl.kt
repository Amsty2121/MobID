package com.example.smartpassuserdevice.data.repository

import android.content.Context
import com.example.smartpassuserdevice.data.model.AccessDto
import com.example.smartpassuserdevice.data.network.AccessApi
import com.example.smartpassuserdevice.util.CacheUtils

class AccessRepositoryImpl(
    private val api: AccessApi,
    private val ctx: Context
) {
    suspend fun getAccesses(): List<AccessDto> {
        val userId = CacheUtils.getUserId(ctx)
        val token = CacheUtils.getToken(ctx)
            ?: return emptyList()
        return try {
            api.getAccesses(userId as String, token)
        } catch (e: Exception) {
            emptyList()
        }
    }
}