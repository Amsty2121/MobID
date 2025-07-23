// src/main/java/com/example/smartpassuserdevice/presentation/home/HomeScreen.kt
@file:OptIn(androidx.camera.core.ExperimentalGetImage::class)

package com.example.smartpassuserdevice.presentation.home

import android.widget.Toast
import androidx.activity.compose.BackHandler
import androidx.camera.core.ExperimentalGetImage
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.FabPosition
import androidx.compose.material3.Scaffold
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.smartpassuserdevice.data.repository.AccessRepositoryImpl
import com.example.smartpassuserdevice.data.repository.ScanRepositoryImpl
import com.example.smartpassuserdevice.presentation.components.BottomBar
import com.example.smartpassuserdevice.presentation.components.FabMenu
import com.example.smartpassuserdevice.presentation.components.TopBar
import com.example.smartpassuserdevice.util.CacheUtils
import kotlinx.coroutines.launch

@androidx.annotation.OptIn(ExperimentalGetImage::class)
@OptIn(ExperimentalGetImage::class)
@Composable
fun HomeScreen(
    accessRepo: AccessRepositoryImpl,
    scanRepo:   ScanRepositoryImpl,
    onLogout:   () -> Unit,
) {
    // Intercept hardware Back
    BackHandler { onLogout() }

    val ctx    = LocalContext.current

    // **Safe-check** for userId
    val userId = CacheUtils.getUserId(ctx)
    if (userId == null) {
        // If missing, immediately trigger logout and don't render the rest
        LaunchedEffect(Unit) { onLogout() }
        return
    }

    // Dialog flags
    var showQr   by remember { mutableStateOf(false) }
    var showScan by remember { mutableStateOf(false) }

    // Scope for launching coroutines
    val scope = rememberCoroutineScope()

    val navController = rememberNavController()
    Scaffold(
        topBar    = { TopBar(onLogout) },
        bottomBar = { BottomBar(navController) },
        floatingActionButtonPosition = FabPosition.End,
        floatingActionButton = {
            // Move FAB into a Box, bottom-end
            Box(
                Modifier
                    .fillMaxWidth()
                    .padding(end = 16.dp),
                contentAlignment = Alignment.BottomEnd
            ) {
                FabMenu(
                    onShowQr = { showQr = true },
                    onScan   = { showScan = true }
                )
            }
        }
    ) { padding ->
        NavHost(
            navController    = navController,
            startDestination = "accesses",
            modifier         = Modifier.padding(padding)
        ) {
            composable("accesses") { AccessesScreen(repo = accessRepo) }
            composable("scans")    { ScansScreen(repo = scanRepo)   }
        }
    }

    if (showQr) {
        QrDialog(
            userId    = userId,
            onDismiss = { showQr = false }
        )
    }

    if (showScan) {
        ScannerDialog(
            onDismiss = { showScan = false },
            onScanned = { raw ->
                scope.launch {
                    try {
                        val result = scanRepo.createScan(raw)
                        Toast.makeText(ctx, "Scanned OK: ${result.id}", Toast.LENGTH_SHORT).show()
                    } catch (e: Exception) {
                        Toast.makeText(ctx, "Eroare scan: ${e.message}", Toast.LENGTH_LONG).show()
                    } finally {
                        showScan = false
                    }
                }
            }
        )
    }
}
