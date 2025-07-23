// src/main/java/com/example/smartpassuserdevice/presentation/navigation/AppNavigation.kt
package com.example.smartpassuserdevice.presentation.navigation

import androidx.activity.ComponentActivity
import androidx.compose.runtime.Composable
import androidx.compose.ui.platform.LocalContext
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.smartpassuserdevice.data.repository.AccessRepositoryImpl
import com.example.smartpassuserdevice.data.repository.AuthRepositoryImpl
import com.example.smartpassuserdevice.data.repository.ScanRepositoryImpl
import com.example.smartpassuserdevice.presentation.login.LoginScreen
import com.example.smartpassuserdevice.presentation.register.RegisterScreen
import com.example.smartpassuserdevice.presentation.home.HomeScreen
import com.example.smartpassuserdevice.util.CacheUtils

@Composable
fun AppNavigation(
    authRepository: AuthRepositoryImpl,
    accessRepository: AccessRepositoryImpl,
    scanRepository: ScanRepositoryImpl
) {
    val nav = rememberNavController()
    val crt = LocalContext.current
    NavHost(nav, startDestination = "login") {
        composable("login") {
            LoginScreen(
                authRepository    = authRepository,
                onLoginSuccess    = { nav.navigate("home") },
                onRegisterClicked = { nav.navigate("register") }
            )
        }
        composable("register") {
            RegisterScreen(
                authRepository     = authRepository,
                onRegisterSuccess  = { nav.popBackStack() },
                onBackToLogin      = { nav.popBackStack() }
            )
        }
        composable("home") {
            HomeScreen(
                accessRepo = accessRepository,
                scanRepo   = scanRepository,
                onLogout   = {
                    CacheUtils.clearAuthData(crt)
                    nav.navigate("login") {
                        popUpTo("home") { inclusive = true }
                    }
                }
            )
        }
    }
}
