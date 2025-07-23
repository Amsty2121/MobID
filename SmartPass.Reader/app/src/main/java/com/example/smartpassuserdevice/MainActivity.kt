package com.example.smartpassuserdevice

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import com.example.smartpassuserdevice.data.network.ApiServiceFactory
import com.example.smartpassuserdevice.data.repository.AccessRepositoryImpl
import com.example.smartpassuserdevice.data.repository.AuthRepositoryImpl
import com.example.smartpassuserdevice.data.repository.ScannerRepositoryImpl
import com.example.smartpassuserdevice.ui.theme.SmartPassUserDeviceTheme
import com.example.smartpassuserdevice.presentation.navigation.AppNavigation

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        val apiFactory = ApiServiceFactory(this)
        val authRepo   = AuthRepositoryImpl(apiFactory.createAuthApi(), this)
        val scannerRepo   = ScannerRepositoryImpl(apiFactory.createScannerApi())

        setContent {
            SmartPassUserDeviceTheme {
                AppNavigation(
                    authRepository   = authRepo,
                    scannerRepository   = scannerRepo
                )
            }
        }
    }
}
