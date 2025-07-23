// src/main/java/com/example/smartpassuserdevice/presentation/home/ScansScreen.kt
package com.example.smartpassuserdevice.presentation.home

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.example.smartpassuserdevice.data.model.ScanDto
import com.example.smartpassuserdevice.data.repository.ScanRepositoryImpl

@Composable
fun ScansScreen(
    repo: ScanRepositoryImpl
) {
    // create a ViewModel that will load scans once
    val vm = remember { ScansViewModel(repo) }
    // observe the flow of scans
    val scans by vm.scans.collectAsState()

    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.surface),
        contentPadding = PaddingValues(16.dp)
    ) {
        if (scans.isEmpty()) {
            item {
                Text(
                    text = "Nu există scanări.",
                    style = MaterialTheme.typography.bodyLarge,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(16.dp)
                )
            }
        } else {
            items(scans) { scan ->
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(vertical = 8.dp),
                    elevation = CardDefaults.cardElevation(defaultElevation = 4.dp)
                ) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("ID: ${scan.id}", style = MaterialTheme.typography.bodyMedium)
                        Spacer(Modifier.height(4.dp))
                        Text("QR Code ID: ${scan.qrCodeId}", style = MaterialTheme.typography.bodySmall)
                        Spacer(Modifier.height(4.dp))
                        Text("Scanat pentru: ${scan.scannedForUserName}", style = MaterialTheme.typography.bodySmall)
                        Spacer(Modifier.height(4.dp))
                        Text("La: ${scan.scannedAt}", style = MaterialTheme.typography.bodySmall)
                    }
                }
            }
        }
    }
}
