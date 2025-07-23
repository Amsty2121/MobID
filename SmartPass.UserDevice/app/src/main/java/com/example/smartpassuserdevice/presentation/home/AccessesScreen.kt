package com.example.smartpassuserdevice.presentation.home

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.KeyboardArrowDown
import androidx.compose.material.icons.filled.KeyboardArrowUp
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.example.smartpassuserdevice.data.model.AccessDto
import com.example.smartpassuserdevice.data.repository.AccessRepositoryImpl
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.time.format.DateTimeParseException

@Composable
fun AccessesScreen(
    repo: AccessRepositoryImpl
) {
    val vm    = remember { AccessesViewModel(repo) }
    val list by vm.accesses.collectAsState()

    // Group by organization
    val grouped = remember(list) { list.groupBy { it.organizationName } }

    // Track expanded state per group; default each to true
    val expandedStates = remember { mutableStateMapOf<String, Boolean>() }
    LaunchedEffect(grouped.keys) {
        grouped.keys.forEach { orgName ->
            if (!expandedStates.containsKey(orgName)) {
                expandedStates[orgName] = true
            }
        }
    }

    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.surface),
        contentPadding = PaddingValues(16.dp)
    ) {
        grouped.forEach { (orgName, accesses) ->
            // Header row
            item {
                val isExpanded = expandedStates[orgName] == true
                Row(
                    Modifier
                        .fillMaxWidth()
                        .clickable {
                            expandedStates[orgName] = !isExpanded
                        }
                        .background(MaterialTheme.colorScheme.primaryContainer)
                        .padding(12.dp),
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Text(
                        text = "$orgName – accesses: ${accesses.size}",
                        style = MaterialTheme.typography.titleMedium,
                        color = MaterialTheme.colorScheme.onPrimaryContainer
                    )
                    Icon(
                        imageVector = if (isExpanded)
                            Icons.Default.KeyboardArrowUp
                        else
                            Icons.Default.KeyboardArrowDown,
                        contentDescription = null,
                        tint = MaterialTheme.colorScheme.onPrimaryContainer
                    )
                }
            }

            // Content cards
            if (expandedStates[orgName] == true) {
                items(accesses) { access ->
                    Card(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(vertical = 8.dp),
                        elevation = CardDefaults.cardElevation(defaultElevation = 4.dp)
                    ) {
                        Column(modifier = Modifier.padding(16.dp)) {
                            Text("Nume: ${access.name}", style = MaterialTheme.typography.bodyLarge)
                            Text("Tip: ${access.accessTypeName}", style = MaterialTheme.typography.bodyMedium)
                            Text("Organizație: ${access.organizationName}", style = MaterialTheme.typography.bodyMedium)
                            Text("Multiscan: ${if (access.isMultiScan) "Da" else "Nu"}", style = MaterialTheme.typography.bodyMedium)
                            Text(
                                "Expiră: " + (access.expirationDateTime?.let {
                                    try {
                                        LocalDateTime.parse(it)
                                            .format(DateTimeFormatter.ofPattern("dd.MM.yyyy"))
                                    } catch (_: Exception) { it }
                                } ?: "Nelimitat"),
                                style = MaterialTheme.typography.bodyMedium
                            )
                            Text("Doar membri: ${if (access.restrictToOrgMembers) "Da" else "Nu"}", style = MaterialTheme.typography.bodyMedium)
                            Text("Partajabil: ${if (access.restrictToOrgSharing) "Da" else "Nu"}", style = MaterialTheme.typography.bodyMedium)
                            Text("Activ: ${if (access.isActive) "Da" else "Nu"}", style = MaterialTheme.typography.bodyMedium)
                        }
                    }
                }
            }
        }
    }
}

