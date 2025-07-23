@file:OptIn(ExperimentalMaterial3Api::class, ExperimentalGetImage::class)

package com.example.smartpassuserdevice.presentation.home

import android.content.Context
import androidx.activity.compose.BackHandler
import androidx.camera.core.ExperimentalGetImage
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.material.ContentAlpha
import androidx.compose.material.ExperimentalMaterialApi
import androidx.compose.material3.*
import androidx.compose.material3.ExposedDropdownMenuDefaults.TrailingIcon
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.smartpassuserdevice.data.repository.ScannerRepositoryImpl
import com.example.smartpassuserdevice.presentation.components.TopBar
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.ui.platform.LocalContext

@ExperimentalGetImage
@ExperimentalMaterial3Api
@OptIn(ExperimentalMaterialApi::class, ExperimentalGetImage::class)
@Composable
fun HomeScreen(
    scannerRepository: ScannerRepositoryImpl,
    onLogout: () -> Unit,
) {
    val vm: HomeViewModel = viewModel(factory = HomeViewModelFactory(scannerRepository))
    val state by vm.uiState.collectAsState()
    val colors = MaterialTheme.colorScheme

    LaunchedEffect(Unit) { vm.loadOrgs() }

    Scaffold(
        containerColor = colors.surface,
        topBar = { TopBar(onLogout = onLogout) }
    ) { padding ->
        Column(
            Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(16.dp)
        ) {
            // 1) Dropdown organizații
            var orgExpanded by remember { mutableStateOf(false) }
            ExposedDropdownMenuBox(
                expanded = orgExpanded,
                onExpandedChange = { orgExpanded = !orgExpanded }
            ) {
                OutlinedTextField(
                    value = state.selectedOrg?.name.orEmpty(),
                    onValueChange = {},
                    readOnly = true,
                    label = { Text("Organizație") },
                    trailingIcon = { TrailingIcon(expanded = orgExpanded) },
                    modifier = Modifier
                        .fillMaxWidth()
                        .menuAnchor(),
                    colors = OutlinedTextFieldDefaults.colors(
                        focusedBorderColor = colors.primary,
                        unfocusedBorderColor = colors.outline,
                        cursorColor = colors.primary,
                        focusedTextColor = colors.onSurface,
                        unfocusedTextColor = colors.onSurface,
                        disabledTextColor = colors.onSurface.copy(alpha = ContentAlpha.disabled),
                        focusedContainerColor = colors.surface,
                        unfocusedContainerColor = colors.surface,
                        disabledContainerColor = colors.surface.copy(alpha = 0.12f)
                    )
                )
                ExposedDropdownMenu(
                    expanded = orgExpanded,
                    onDismissRequest = { orgExpanded = false },
                    modifier = Modifier.background(colors.secondary)
                ) {
                    state.orgs.forEach { org ->
                        DropdownMenuItem(
                            text = { Text(text = org.name, style = MaterialTheme.typography.bodyLarge) },
                            onClick = {
                                vm.onOrgSelected(org)
                                orgExpanded = false
                            }
                        )
                    }
                }
            }

            Spacer(Modifier.height(16.dp))

            // 2) Dropdown accese
            state.selectedOrg?.let {
                var accExpanded by remember { mutableStateOf(false) }
                ExposedDropdownMenuBox(
                    expanded = accExpanded,
                    onExpandedChange = { accExpanded = !accExpanded }
                ) {
                    OutlinedTextField(
                        value = state.selectedAccess?.name.orEmpty(),
                        onValueChange = {},
                        readOnly = true,
                        label = { Text("Acces") },
                        trailingIcon = { TrailingIcon(expanded = accExpanded) },
                        modifier = Modifier
                            .fillMaxWidth()
                            .menuAnchor(),
                        colors = OutlinedTextFieldDefaults.colors(
                            focusedBorderColor = colors.primary,
                            unfocusedBorderColor = colors.outline,
                            cursorColor = colors.primary,
                            focusedTextColor = colors.onSurface,
                            unfocusedTextColor = colors.onSurface,
                            disabledTextColor = colors.onSurface.copy(alpha = ContentAlpha.disabled),
                            focusedContainerColor = colors.surface,
                            unfocusedContainerColor = colors.surface,
                            disabledContainerColor = colors.surface.copy(alpha = 0.12f)
                        )
                    )

                    ExposedDropdownMenu(
                        expanded = accExpanded,
                        onDismissRequest = { accExpanded = false },
                        modifier = Modifier.background(colors.secondary)
                    ) {
                        state.accesses.forEach { acc ->
                            DropdownMenuItem(
                                text = { Text(text = acc.name, style = MaterialTheme.typography.bodyLarge) },
                                onClick = {
                                    vm.onAccessSelected(acc)
                                    accExpanded = false
                                }
                            )
                        }
                    }
                }
            }

            Spacer(Modifier.height(24.dp))

            // 3) Buton scan
            Button(
                onClick = { vm.startScan() },
                enabled = state.selectedOrg != null && state.selectedAccess != null,
                modifier = Modifier.fillMaxWidth(),
                colors = ButtonDefaults.buttonColors(containerColor = colors.primary)
            ) {
                Text("Scan QR", color = colors.onPrimary)
            }

            // Feedback vizual mare ✔ sau ✖
            state.scanResult?.let {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(top = 32.dp),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        text = if (it.success) "✔" else "✖",
                        color = if (it.success) Color(0xFF2E7D32) else Color(0xFFC62828),
                        fontSize = 64.sp,
                        fontWeight = FontWeight.Bold
                    )
                }
            }

            // Eroare text (opțional)
            state.error?.let { err ->
                Text(text = err, color = colors.error, modifier = Modifier.padding(top = 8.dp))
            }
        }
    }

    if (state.showScannerDialog) {
        ScannerDialog(
            onDismiss = { vm.handleScanned("") },
            onScanned = vm::handleScanned
        )
    }
}
