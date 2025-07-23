package com.example.smartpassuserdevice.presentation.home

import androidx.compose.foundation.layout.padding
import androidx.compose.material3.FabPosition
import androidx.compose.material3.Scaffold
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.smartpassuserdevice.data.repository.AccessRepositoryImpl
import com.example.smartpassuserdevice.data.repository.ScanRepositoryImpl
import com.example.smartpassuserdevice.presentation.components.BottomBar
import com.example.smartpassuserdevice.presentation.components.FabMenu
import com.example.smartpassuserdevice.presentation.components.TopBar

@Composable
fun HomeNavGraph(
    accessRepo: AccessRepositoryImpl,
    scanRepo: ScanRepositoryImpl,
    onLogout:   () -> Unit,
    onShowMyQr: () -> Unit,
    onScan:     () -> Unit
) {
    val nav = rememberNavController()
    Scaffold(
        topBar    = { TopBar(onLogout) },
        bottomBar = { BottomBar(nav) },
        floatingActionButton = {
            FabMenu(onShowQr = onShowMyQr, onScan = onScan)
        },
        floatingActionButtonPosition = FabPosition.Center
    ) { padding ->
        NavHost(
            navController    = nav,
            startDestination = "accesses",
            Modifier.padding(padding)
        ) {
            composable("accesses") { AccessesScreen(accessRepo) }
            composable("scans")    { ScansScreen(scanRepo)   }
        }
    }
}
