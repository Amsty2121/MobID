package com.example.smartpassuserdevice.presentation.home

import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.CreditCard
import androidx.compose.material.icons.filled.QrCodeScanner
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.smartpassuserdevice.data.repository.AccessRepositoryImpl
import com.example.smartpassuserdevice.data.repository.ScanRepositoryImpl
import androidx.compose.material3.MaterialTheme as M3Theme

@Composable
fun HomeWithDockedFab(
    accessRepo: AccessRepositoryImpl,
    scanRepo: ScanRepositoryImpl,
    onAccesses: () -> Unit,
    onScans:    () -> Unit,
    onShowQr:   () -> Unit,
    onScanQr:   () -> Unit,
    onLogout:   () -> Unit
) {
    // culori din tema Material-3
    val colors3 = M3Theme.colorScheme

    Scaffold(
        topBar = {
            // folosești TopBar-ul tău Material-3
            com.example.smartpassuserdevice.presentation.components.TopBar(onLogout)
        },
        floatingActionButton = {
            FloatingActionButton(
                onClick = onShowQr,
                backgroundColor = colors3.primary,
                contentColor    = colors3.onPrimary
            ) {
                Icon(Icons.Default.Add, contentDescription = "Arată/Scanează QR")
            }
        },
        floatingActionButtonPosition = FabPosition.Center,
        isFloatingActionButtonDocked = true,
        bottomBar = {
            BottomAppBar(
                cutoutShape     = CircleShape,
                backgroundColor = colors3.primary,
                contentColor    = colors3.onPrimary,
                elevation       = 8.dp
            ) {
                IconButton(onClick = onAccesses, modifier = Modifier.padding(start = 16.dp)) {
                    Icon(Icons.Default.CreditCard, contentDescription = "Accesses")
                }
                Spacer(Modifier.weight(1f, true))
                IconButton(onClick = onScans, modifier = Modifier.padding(end = 16.dp)) {
                    Icon(Icons.Default.QrCodeScanner, contentDescription = "Scans")
                }
            }
        }
    ) { innerPadding ->
        // aici pui NavHost-ul tău sau direct AccessesScreen/ScansScreen
        NavHost(
            navController = rememberNavController(),
            startDestination = "accesses",
            modifier = Modifier.padding(innerPadding)
        ) {
            composable("accesses") {
                AccessesScreen(repo = accessRepo)
            }
            composable("scans") {
                ScansScreen(repo = scanRepo)
            }
        }
    }
}
