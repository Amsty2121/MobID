package com.example.smartpassuserdevice.presentation.home

import android.util.Base64
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.size
import androidx.compose.material3.Card
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import androidx.compose.ui.window.Dialog
import androidx.compose.foundation.Image
import com.example.smartpassuserdevice.util.generateQrBitmap

@Composable
fun QrDialog(
    userId: String,
    onDismiss: () -> Unit
) {
    // codul pe care-l afișăm în QR
    val payload = "$userId:USERQR"
    // îl transformăm în Base64 (opțional)
    val base64 = Base64.encodeToString(payload.toByteArray(), Base64.NO_WRAP)
    val qrBmp = generateQrBitmap(base64, size = 512)

    Dialog(onDismissRequest = onDismiss) {
        Box(
            Modifier
                .size(280.dp)
                .background(Color.White)
                .clickable { onDismiss() },             // click oriunde în card închide
            contentAlignment = Alignment.Center
        ) {
            Card(
                Modifier
                    .clickable(enabled = false) {}       // împiedică propagarea click-ului în afară
            ) {
                Image(
                    bitmap = qrBmp.asImageBitmap(),
                    contentDescription = "QR Code",
                    modifier = Modifier.size(256.dp)
                )
            }
        }
    }
}
