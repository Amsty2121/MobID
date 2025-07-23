package com.example.smartpassuserdevice.ui.view

import android.content.Context
import android.util.Log
import android.widget.Toast
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Refresh
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.lifecycle.ViewModelStoreOwner
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavHostController
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

@Composable
fun BottomNavigationBar(
    onRefreshClicked: () -> Unit
) {
    NavigationBar(
        containerColor = MaterialTheme.colorScheme.primary,
        modifier = Modifier.padding(top = 2.dp, bottom = 0.dp)
    ) {
        NavigationBarItem(
            selected = false,
            onClick = {
                onRefreshClicked()
            },
            icon = {
                Icon(
                    imageVector = Icons.Filled.Refresh,
                    contentDescription = "Refresh",
                    modifier = Modifier.size(45.dp)
                )
            },
            colors = NavigationBarItemDefaults.colors(
                selectedIconColor = MaterialTheme.colorScheme.secondary,
                unselectedIconColor = MaterialTheme.colorScheme.onPrimary,
                indicatorColor = MaterialTheme.colorScheme.onPrimary
            )
        )
    }
}

/*private fun handleGetCards(
    context: Context,
    userViewModel: UserViewModel,
    onCardsUpdated: (List<MyCard>) -> Unit
) {
    val coroutineScope: CoroutineScope = CoroutineScope(Dispatchers.IO)
    coroutineScope.launch {
        val token = CacheUtils.getTokenFromCache(context)

        if (token != null) {
            // Очистка кэша перед запросом
            withContext(Dispatchers.Main) {
                CacheUtils.removeAllCardsFromCache(context)
                CacheUtils.removeSelectedCardFromCache(context)
                Log.d("CacheUtils", "Saved Cards: ${CacheUtils.getSelectedCardIndexFromCache(context)}")

                onCardsUpdated(emptyList())
            }

            val cardsResult = userViewModel.getMyCards(token)

            withContext(Dispatchers.Main) {
                if (cardsResult.isSuccess) {
                    val cards = cardsResult.getOrNull()
                    if (cards != null) {
                        // Сохранение карт в кэш
                        CacheUtils.saveUserCardsResponseToCache(context, cards)
                        CacheUtils.saveSelectedCardIndexToCache(context, 0)
                        CacheUtils.saveSelectedCardToCache(context, cards.first())
                        Log.d("CacheUtils", "Saved Cards: ${CacheUtils.getUserCardsListFromCache(context)}")

                        Toast.makeText(context, "Cards refreshed successfully", Toast.LENGTH_SHORT).show()

                        // Обновление состояния UI с новыми картами
                        onCardsUpdated(cards)
                    }
                } else {
                    Toast.makeText(context, "Failed to refresh cards", Toast.LENGTH_SHORT).show()
                }
            }
        } else {
            withContext(Dispatchers.Main) {
                Toast.makeText(context, "User not logged in", Toast.LENGTH_SHORT).show()
            }
        }
    }
}

*/