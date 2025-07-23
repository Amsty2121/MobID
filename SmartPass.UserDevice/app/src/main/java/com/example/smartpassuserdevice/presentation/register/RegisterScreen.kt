// src/main/java/com/example/smartpassuserdevice/presentation/register/RegisterScreen.kt
package com.example.smartpassuserdevice.presentation.register

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
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
import com.example.smartpassuserdevice.presentation.register.RegisterViewModel
import com.example.smartpassuserdevice.presentation.register.RegisterViewModelFactory
import com.example.smartpassuserdevice.util.CacheUtils

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun RegisterScreen(
    authRepository: AuthRepositoryImpl,
    onRegisterSuccess: () -> Unit,
    onBackToLogin: () -> Unit
) {
    // Context & theme
    val ctx = LocalContext.current
    val colors = MaterialTheme.colorScheme

    // Dacă găsim token, înapoi la login
    if (CacheUtils.getToken(ctx) != null) {
        LaunchedEffect(Unit) { onBackToLogin() }
        return
    }

    // ViewModel
    val vm: RegisterViewModel = viewModel(
        factory = RegisterViewModelFactory(authRepository)
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
                Text(
                    "Register",
                    fontSize = 22.sp,
                    fontWeight = FontWeight.Bold,
                    color = colors.onSurface
                )
                Spacer(Modifier.height(16.dp))

                // Username
                OutlinedTextField(
                    value = state.username,
                    onValueChange = vm::onUsernameChange,
                    placeholder = { Text("Username", color = colors.onSurface.copy(alpha = 0.5f)) },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth(),
                    textStyle = TextStyle(color = colors.onSurface),
                    shape = RectangleShape,
                    colors = TextFieldDefaults.outlinedTextFieldColors(
                        containerColor       = colors.surface,
                        cursorColor          = colors.primary,
                        focusedBorderColor   = colors.primary,
                        unfocusedBorderColor = colors.onSurface.copy(alpha = 0.5f),
                        focusedLabelColor    = colors.primary,
                        unfocusedLabelColor  = colors.onSurface.copy(alpha = 0.7f)
                    )
                )
                Spacer(Modifier.height(12.dp))

                // Email
                OutlinedTextField(
                    value = state.email,
                    onValueChange = vm::onEmailChange,
                    placeholder = { Text("Email", color = colors.onSurface.copy(alpha = 0.5f)) },
                    singleLine = true,
                    modifier = Modifier.fillMaxWidth(),
                    textStyle = TextStyle(color = colors.onSurface),
                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                    shape = RectangleShape,
                    colors = TextFieldDefaults.outlinedTextFieldColors(
                        containerColor       = colors.surface,
                        cursorColor          = colors.primary,
                        focusedBorderColor   = colors.primary,
                        unfocusedBorderColor = colors.onSurface.copy(alpha = 0.5f),
                        focusedLabelColor    = colors.primary,
                        unfocusedLabelColor  = colors.onSurface.copy(alpha = 0.7f)
                    )
                )
                Spacer(Modifier.height(12.dp))

                // Password
                OutlinedTextField(
                    value = state.password,
                    onValueChange = vm::onPasswordChange,
                    placeholder = { Text("Password", color = colors.onSurface.copy(alpha = 0.5f)) },
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
                    shape = RectangleShape,
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

                // Register button
                Button(
                    onClick = vm::register,
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
                        Text("Register")
                    }
                }
                Spacer(Modifier.height(12.dp))

                // Back to Login
                OutlinedButton(
                    onClick = onBackToLogin,
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(48.dp),
                    colors = ButtonDefaults.outlinedButtonColors(
                        contentColor = colors.primary
                    ),
                    border = ButtonDefaults.outlinedButtonBorder.copy(
                        brush = SolidColor(colors.primary)
                    ),
                    shape = RectangleShape
                ) {
                    Text("Back to Login")
                }

                state.error?.let { err ->
                    Spacer(Modifier.height(16.dp))
                    Text(err, color = colors.error)
                }
            }
        }
    }

    // La succes
    LaunchedEffect(state.success) {
        if (state.success) onRegisterSuccess()
    }
}
