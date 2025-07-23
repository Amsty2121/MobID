package com.example.smartpassuserdevice.data.network

import android.content.Context
import com.example.smartpassuserdevice.util.CacheUtils
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import okhttp3.Interceptor
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.moshi.MoshiConverterFactory
import java.text.SimpleDateFormat
import java.util.*
import java.util.concurrent.TimeUnit

class DateJsonAdapter {
    private val fmt = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US)
    @com.squareup.moshi.ToJson
    fun toJson(d: Date): String = fmt.format(d)

    @com.squareup.moshi.FromJson
    fun fromJson(s: String): Date = fmt.parse(s)!!
}

class UUIDJsonAdapter {
    @com.squareup.moshi.ToJson
    fun toJson(u: UUID): String = u.toString()

    @com.squareup.moshi.FromJson
    fun fromJson(s: String): UUID = UUID.fromString(s)
}

class ApiServiceFactory(private val ctx: Context) {
    companion object {
        private const val BASE_URL = "http://192.168.32.218:5287/api/"
        //private const val BASE_URL = "http://10.0.2.2:5287/api/"
    }

    private val authInterceptor = Interceptor { chain ->
        val req = chain.request()
        val builder = req.newBuilder()
        // nu adăugăm token pe endpoint-urile /auth
        if (!req.url.encodedPath.startsWith("/auth")) {
            CacheUtils.getToken(ctx)
                ?.takeIf { it.isNotBlank() }
                ?.let { builder.header("Authorization", it) }
        }
        chain.proceed(builder.build())
    }

    private val moshi = Moshi.Builder()
        .add(DateJsonAdapter())
        .add(KotlinJsonAdapterFactory())
        .add(UUIDJsonAdapter())
        .build()

    private val client = OkHttpClient.Builder()
        .addInterceptor(authInterceptor)
        .addInterceptor(HttpLoggingInterceptor().apply { level = HttpLoggingInterceptor.Level.BODY })
        .connectTimeout(30, TimeUnit.SECONDS)
        .readTimeout(30, TimeUnit.SECONDS)
        .writeTimeout(30, TimeUnit.SECONDS)
        .build()

    private val retrofit = Retrofit.Builder()
        .baseUrl(BASE_URL)
        .client(client)
        .addConverterFactory(MoshiConverterFactory.create(moshi))
        .build()

    fun createAuthApi(): AuthApi     = retrofit.create(AuthApi::class.java)
    fun createAccessApi(): AccessApi = retrofit.create(AccessApi::class.java)
    fun createScannerApi(): ScannerApi     = retrofit.create(ScannerApi::class.java)
}
