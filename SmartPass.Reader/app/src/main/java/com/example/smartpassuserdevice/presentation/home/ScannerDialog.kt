// src/main/java/com/example/smartpassuserdevice/presentation/home/ScannerDialog.kt
package com.example.smartpassuserdevice.presentation.home

import android.Manifest
import android.annotation.SuppressLint
import android.content.pm.PackageManager
import android.util.Log
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.camera.core.CameraSelector
import androidx.camera.core.ExperimentalGetImage
import androidx.camera.core.ImageAnalysis
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.camera.view.PreviewView
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.platform.LocalLifecycleOwner
import androidx.compose.ui.unit.dp
import androidx.compose.ui.viewinterop.AndroidView
import androidx.compose.ui.window.Dialog
import androidx.core.content.ContextCompat
import com.google.mlkit.vision.barcode.common.Barcode
import com.google.mlkit.vision.barcode.BarcodeScannerOptions
import com.google.mlkit.vision.barcode.BarcodeScanning
import com.google.mlkit.vision.common.InputImage

@ExperimentalGetImage
@SuppressLint("UnsafeOptInUsageError")
@Composable
fun ScannerDialog(
    onDismiss: () -> Unit,
    onScanned: (String) -> Unit
) {
    val context = LocalContext.current
    val lifecycleOwner = LocalLifecycleOwner.current

    // 1) Permisiune cameră
    var hasCameraPermission by remember {
        mutableStateOf(
            ContextCompat.checkSelfPermission(context, Manifest.permission.CAMERA)
                    == PackageManager.PERMISSION_GRANTED
        )
    }
    val permissionLauncher = rememberLauncherForActivityResult(
        ActivityResultContracts.RequestPermission()
    ) { granted ->
        hasCameraPermission = granted
    }
    LaunchedEffect(Unit) {
        if (!hasCameraPermission) {
            permissionLauncher.launch(Manifest.permission.CAMERA)
        }
    }

    // 2) ML Kit QR-only scanner
    val qrScanner = remember {
        val options = BarcodeScannerOptions.Builder()
            .setBarcodeFormats(Barcode.FORMAT_QR_CODE)
            .build()
        BarcodeScanning.getClient(options)
    }

    // 3) Flag de unică detecție
    var scanned by remember { mutableStateOf(false) }

    // 4) Referință la cameraProvider
    var cameraProvider by remember { mutableStateOf<ProcessCameraProvider?>(null) }

    // 5) Unbind când dialogul dispare
    DisposableEffect(Unit) {
        onDispose {
            cameraProvider?.unbindAll()
            Log.d("ScannerDialog", "Camera unbound on dialog dispose")
        }
    }

    Dialog(onDismissRequest = {
        scanned = true
        onDismiss()
    }) {
        Surface(modifier = Modifier.fillMaxSize()) {
            if (!hasCameraPermission) {
                Text(
                    "Permite accesul la cameră pentru scanare.",
                    Modifier
                        .padding(16.dp)
                        .fillMaxWidth()
                )
            } else {
                AndroidView(
                    modifier = Modifier.fillMaxSize(),
                    factory = { ctx ->
                        PreviewView(ctx).apply {
                            val future = ProcessCameraProvider.getInstance(ctx)
                            future.addListener({
                                val provider = future.get()
                                cameraProvider = provider

                                // Preview
                                val previewUseCase = androidx.camera.core.Preview.Builder()
                                    .build()
                                    .also { it.setSurfaceProvider(surfaceProvider) }

                                // Analysis
                                val analysisUseCase = ImageAnalysis.Builder()
                                    .setBackpressureStrategy(ImageAnalysis.STRATEGY_KEEP_ONLY_LATEST)
                                    .build()
                                    .also { useCase ->
                                        useCase.setAnalyzer(
                                            ContextCompat.getMainExecutor(ctx)
                                        ) { imageProxy ->
                                            if (scanned) {
                                                imageProxy.close()
                                                return@setAnalyzer
                                            }
                                            val mediaImage = imageProxy.image
                                            if (mediaImage != null) {
                                                val image = InputImage.fromMediaImage(
                                                    mediaImage,
                                                    imageProxy.imageInfo.rotationDegrees
                                                )
                                                qrScanner.process(image)
                                                    .addOnSuccessListener { barcodes ->
                                                        barcodes.firstOrNull()?.rawValue
                                                            ?.let { code ->
                                                                scanned = true
                                                                onScanned(code)
                                                            }
                                                    }
                                                    .addOnCompleteListener {
                                                        imageProxy.close()
                                                    }
                                            } else {
                                                imageProxy.close()
                                            }
                                        }
                                    }

                                try {
                                    provider.unbindAll()
                                    provider.bindToLifecycle(
                                        lifecycleOwner,
                                        CameraSelector.DEFAULT_BACK_CAMERA,
                                        previewUseCase,
                                        analysisUseCase
                                    )
                                } catch (e: Exception) {
                                    Log.e("ScannerDialog", "Camera bind failed", e)
                                }
                            }, ContextCompat.getMainExecutor(ctx))
                        }
                    }
                )
            }
        }
    }
}
