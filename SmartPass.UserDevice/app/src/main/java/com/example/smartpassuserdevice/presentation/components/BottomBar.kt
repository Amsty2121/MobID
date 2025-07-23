package com.example.smartpassuserdevice.presentation.components

import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.CreditCard
import androidx.compose.material.icons.filled.QrCodeScanner
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.NavigationBar
import androidx.compose.material3.NavigationBarDefaults
import androidx.compose.material3.NavigationBarItem
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import androidx.navigation.compose.currentBackStackEntryAsState

@Composable
fun BottomBar(nav: NavController) {
    val colors  = MaterialTheme.colorScheme
    val backStack = nav.currentBackStackEntryAsState()
    val current   = backStack.value?.destination?.route

    NavigationBar(
        containerColor = colors.primary,
        contentColor   = colors.onPrimary
    ) {
        NavigationBarItem(
            selected = current == "accesses",
            onClick  = { nav.navigate("accesses") { launchSingleTop = true } },
            icon     = { Icon(Icons.Default.CreditCard, contentDescription = "Accesses") },
            label    = { Text("Accesses") }
        )
        NavigationBarItem(
            selected = current == "scans",
            onClick  = { nav.navigate("scans")    { launchSingleTop = true } },
            icon     = { Icon(Icons.Default.QrCodeScanner, contentDescription = "Scans") },
            label    = { Text("Scans") }
        )
    }
}
