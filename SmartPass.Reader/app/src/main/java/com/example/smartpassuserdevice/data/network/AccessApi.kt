package com.example.smartpassuserdevice.data.network

import com.example.smartpassuserdevice.data.model.AccessDto
import retrofit2.http.GET
import retrofit2.http.Header
import retrofit2.http.Path

interface AccessApi {
    @GET("user/{userId}/all-accesses")
    suspend fun getAccesses(
        @Path("userId") userId: String,
        @Header("Authorization") token: String
    ): List<AccessDto>
}
