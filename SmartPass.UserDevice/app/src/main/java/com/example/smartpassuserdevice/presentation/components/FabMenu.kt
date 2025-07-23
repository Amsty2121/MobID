// src/main/java/com/example/smartpassuserdevice/presentation/components/FabMenu.kt
package com.example.smartpassuserdevice.presentation.components

import androidx.compose.animation.*
import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.QrCode
import androidx.compose.material.icons.filled.QrCodeScanner
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@OptIn(ExperimentalAnimationApi::class, ExperimentalMaterial3Api::class)
@Composable
fun FabMenu(
    onShowQr: () -> Unit,
    onScan:   () -> Unit
) {
    var expanded by remember { mutableStateOf(false) }

    // Column plasată în slot-ul floatingActionButton al Scaffold-ului
    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.spacedBy(12.dp),
        modifier = Modifier
            // ajustează distanța față de bottom bar și marginile laterale
            .padding(bottom = 40.dp)
    ) {
        // mini-FAB pentru "Arată QR"
        AnimatedVisibility(
            visible = expanded,
            enter = fadeIn() + scaleIn(),
            exit  = fadeOut() + scaleOut()
        ) {
            SmallFloatingActionButton(
                onClick = {
                    expanded = false
                    onShowQr()
                },
                containerColor = MaterialTheme.colorScheme.primary,
                contentColor   = MaterialTheme.colorScheme.onPrimary
            ) {
                Icon(Icons.Default.QrCode, contentDescription = "Arată QR")
            }
        }

        // mini-FAB pentru "Scanează QR"
        AnimatedVisibility(
            visible = expanded,
            enter = fadeIn() + scaleIn(),
            exit  = fadeOut() + scaleOut()
        ) {
            SmallFloatingActionButton(
                onClick = {
                    expanded = false
                    onScan()
                },
                containerColor = MaterialTheme.colorScheme.primary,
                contentColor   = MaterialTheme.colorScheme.onPrimary
            ) {
                Icon(Icons.Default.QrCodeScanner, contentDescription = "Scanează QR")
            }
        }

        // butonul principal de toggle
        FloatingActionButton(
            onClick = { expanded = !expanded },
            containerColor = MaterialTheme.colorScheme.primary,
            contentColor   = MaterialTheme.colorScheme.onPrimary
        ) {
            Icon(Icons.Default.Add, contentDescription = "Meniu")
        }
    }
}
