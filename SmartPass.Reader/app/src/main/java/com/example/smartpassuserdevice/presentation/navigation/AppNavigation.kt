// src/main/java/com/example/smartpassuserdevice/presentation/navigation/AppNavigation.kt
@file:kotlin.OptIn(ExperimentalMaterial3Api::class)

package com.example.smartpassuserdevice.presentation.navigation

import android.content.Context
import androidx.annotation.OptIn
import androidx.camera.core.ExperimentalGetImage
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.runtime.Composable
import androidx.compose.ui.platform.LocalContext
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.smartpassuserdevice.data.repository.AuthRepositoryImpl
import com.example.smartpassuserdevice.data.repository.ScannerRepositoryImpl
import com.example.smartpassuserdevice.presentation.login.LoginScreen
import com.example.smartpassuserdevice.presentation.home.HomeScreen
import com.example.smartpassuserdevice.util.CacheUtils

@OptIn(ExperimentalGetImage::class)
@Composable
fun AppNavigation(
    authRepository: AuthRepositoryImpl,
    scannerRepository: ScannerRepositoryImpl,
) {
    val navController = rememberNavController()
    val context = LocalContext.current as Context

    NavHost(
        navController = navController,
        startDestination = "login"
    ) {
        composable("login") {
            LoginScreen(
                authRepository = authRepository,
                onLoginSuccess = {
                    navController.navigate("home") {
                        popUpTo("login") { inclusive = true }
                    }
                }
            )
        }

        composable("home") {
            HomeScreen(
                scannerRepository = scannerRepository,
                onLogout = {
                    CacheUtils.clearAuthData(context)
                    navController.navigate("login") {
                        popUpTo("home") { inclusive = true }
                    }
                }
            )
        }
    }
}
