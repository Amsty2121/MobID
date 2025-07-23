package com.example.smartpassuserdevice.ui.theme

import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color

// Echivalente CSS:
// --color-primary:    #FF9800
// --color-shark:      #32353E
// --color-tuna:       #202733
// --color-tuna-light: #F2F5F7
// --color-iron:       #CACED2
// --color-regent-gray:#5F6C7B
// --color-error:      #D32F2F

private val AppColorScheme = lightColorScheme(
    primary        = Color(0xFFFF9800), // color-primary
    onPrimary      = Color(0xFF32353E), // color-shark
    secondary      = Color(0xFF32353E), // color-shark
    tertiary       = Color(0xFF3C3E47), // a treia nuanță din theme
    background     = Color(0xFFF2F5F7), // color-tuna-light
    onBackground   = Color(0xFF202733), // color-tuna
    surface        = Color(0xFF202733), // color-tuna
    onSurface      = Color(0xFFCACED2), // color-iron
    error          = Color(0xFFD32F2F)  // color-error
)

@Composable
fun SmartPassUserDeviceTheme(
    content: @Composable () -> Unit
) {
    MaterialTheme(
        colorScheme = AppColorScheme,
        typography  = Typography,
        content     = content
    )
}
