// src/main/java/com/example/smartpassuserdevice/presentation/login/LoginScreen.kt
package com.example.smartpassuserdevice.presentation.login

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.Visibility
import androidx.compose.material.icons.filled.VisibilityOff
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.RectangleShape
import androidx.compose.ui.graphics.SolidColor
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.*
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.smartpassuserdevice.data.repository.AuthRepositoryImpl
import com.example.smartpassuserdevice.presentation.login.LoginViewModel
import com.example.smartpassuserdevice.presentation.login.LoginViewModelFactory
import com.example.smartpassuserdevice.util.CacheUtils

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun LoginScreen(
    authRepository: AuthRepositoryImpl,
    onLoginSuccess:    () -> Unit,
) {
    val ctx = LocalContext.current

    // Dacă avem deja token valid, navigăm direct mai departe
    if (CacheUtils.getToken(ctx) != null) {
        LaunchedEffect(Unit) { onLoginSuccess() }
        return
    }

    val colors = MaterialTheme.colorScheme

    // ViewModel scoped to this screen
    val vm: LoginViewModel = viewModel(
        factory = LoginViewModelFactory(authRepository)
    )
    val state by vm.uiState.collectAsState()

    var showPassword by remember { mutableStateOf(false) }

    Box(
        Modifier
            .fillMaxSize()
            .background(colors.surface),
        contentAlignment = Alignment.Center
    ) {
        Card(
            colors = CardDefaults.cardColors(containerColor = colors.surface),
            shape  = RectangleShape,
            modifier = Modifier
                .widthIn(max = 360.dp)
                .padding(16.dp)
        ) {
            Column(
                Modifier
                    .fillMaxWidth()
                    .padding(24.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Icon(
                    Icons.Default.Lock,
                    contentDescription = null,
                    tint = colors.primary,
                    modifier = Modifier.size(48.dp)
                )
                Spacer(Modifier.height(8.dp))
                Text(
                    "Autentificare",
                    fontSize = 22.sp,
                    fontWeight = FontWeight.Bold,
                    color = colors.onSurface
                )
                Text(
                    "Introduceți datele contului pentru a continua",
                    fontSize = 14.sp,
                    color = colors.onSurface.copy(alpha = 0.7f)
                )
                Spacer(Modifier.height(24.dp))

                // Email
                OutlinedTextField(
                    value = state.email,
                    onValueChange = vm::onEmailChange,
                    placeholder = { Text("exemplu@mail.com", color = colors.onSurface.copy(alpha = 0.5f)) },
                    singleLine = true,
                    modifier   = Modifier.fillMaxWidth(),
                    textStyle  = TextStyle(color = colors.onSurface),
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                    colors = TextFieldDefaults.outlinedTextFieldColors(
                        containerColor       = colors.surface,
                        cursorColor          = colors.primary,
                        focusedBorderColor   = colors.primary,
                        unfocusedBorderColor = colors.onSurface.copy(alpha = 0.5f),
                        focusedLabelColor    = colors.primary,
                        unfocusedLabelColor  = colors.onSurface.copy(alpha = 0.7f)
                    )
                )
                Spacer(Modifier.height(16.dp))

                // Password
                OutlinedTextField(
                    value = state.password,
                    onValueChange = vm::onPasswordChange,
                    placeholder = { Text("••••••••", color = colors.onSurface.copy(alpha = 0.5f)) },
                    singleLine = true,
                    visualTransformation = if (showPassword) VisualTransformation.None else PasswordVisualTransformation(),
                    trailingIcon = {
                        IconButton(onClick = { showPassword = !showPassword }) {
                            Icon(
                                imageVector = if (showPassword) Icons.Default.Visibility else Icons.Default.VisibilityOff,
                                contentDescription = null,
                                tint = colors.primary
                            )
                        }
                    },
                    modifier = Modifier.fillMaxWidth(),
                    textStyle = TextStyle(color = colors.onSurface),
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Password),
                    colors = TextFieldDefaults.outlinedTextFieldColors(
                        containerColor       = colors.surface,
                        cursorColor          = colors.primary,
                        focusedBorderColor   = colors.primary,
                        unfocusedBorderColor = colors.onSurface.copy(alpha = 0.5f),
                        focusedLabelColor    = colors.primary,
                        unfocusedLabelColor  = colors.onSurface.copy(alpha = 0.7f)
                    )
                )
                Spacer(Modifier.height(24.dp))

                // Login
                Button(
                    onClick = vm::login,
                    enabled = !state.isLoading,
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(48.dp),
                    colors = ButtonDefaults.buttonColors(
                        containerColor = colors.primary,
                        contentColor   = colors.onPrimary
                    ),
                    shape = RectangleShape
                ) {
                    if (state.isLoading) {
                        CircularProgressIndicator(
                            Modifier.size(24.dp),
                            color = colors.onPrimary,
                            strokeWidth = 2.dp
                        )
                    } else {
                        Text("Autentificare")
                    }
                }

                state.error?.let { err ->
                    Spacer(Modifier.height(16.dp))
                    Text(err, color = colors.error)
                }
            }
        }
    }

    LaunchedEffect(state.success) {
        if (state.success && state.loginResponse != null) {
            CacheUtils.saveLoginResponse(ctx, state.loginResponse)
            onLoginSuccess()
        }
    }
}