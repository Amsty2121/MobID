package com.example.smartpassuserdevice.presentation.home

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.example.smartpassuserdevice.data.model.AccessDto
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.time.format.DateTimeParseException

@Composable
fun AccessItem(
    access: AccessDto,
    onClick: (() -> Unit)? = null
) {
    // Format expirationDateTime (ISO) → "dd.MM.yyyy"
    val expirationText = access.expirationDateTime?.let { iso ->
        try {
            LocalDateTime.parse(iso).format(DateTimeFormatter.ofPattern("dd.MM.yyyy"))
        } catch (e: DateTimeParseException) {
            iso  // fallback
        }
    } ?: "Nelimitat"

    Row(
        Modifier
            .fillMaxWidth()
            .clickable { onClick?.invoke() }
            .padding(vertical = 12.dp, horizontal = 16.dp)
    ) {
        Column {
            Text(
                text = access.name,
                style = MaterialTheme.typography.titleMedium
            )
            Spacer(Modifier.height(4.dp))
            Text(
                text = "Expiră: $expirationText",
                style = MaterialTheme.typography.bodyMedium
            )
        }
    }
}
