package com.example.smartpassuserdevice.presentation.home

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.example.smartpassuserdevice.data.model.ScanDto

@Composable
fun ScanItem(scan: ScanDto) {
    Row(
        Modifier
            .fillMaxWidth()
            .clickable { /* dacă doreşti detalii */ }
            .padding(16.dp)
    ) {
        Column {
            Text("Scan la: ${scan.scannedAt}")
            Spacer(Modifier.height(4.dp))
            //Text("Access: ${scan.accessName}")
        }
    }
}
