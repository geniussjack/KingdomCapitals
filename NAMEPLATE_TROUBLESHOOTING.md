# Проблема с исчезновением названий поселений - Диагностика

## Ситуация
Все названия поселений исчезают с карты кампании, хотя:
- Все UI патчи ОТКЛЮЧЕНЫ (SettlementNameplatesVMPatch, SettlementNameplateViewPatch)
- Логи показывают "Total methods patched: 0" до последнего обновления
- Проверка целостности файлов игры прошла успешно

## Возможные причины

### 1. XML файлы конфликтуют с игрой
Хотя XML файлы (settlements.xml, SpriteData.xml) не должны влиять на отображение названий, попробуем их отключить.

### 2. Другие моды
Возможно конфликт с другим модом.

### 3. Баг игры версии 1.2.12
В игре есть баги с исчезновением названий при зуме.

## Тест 1: Запуск БЕЗ модов

1. Отключите ВСЕ моды в лаунчере
2. Запустите игру
3. Загрузите сохранение или начните новую игру
4. Проверьте, отображаются ли названия поселений

**Если названия исчезают БЕЗ модов** → Это баг игры, не мода!

## Тест 2: Отключение XML файлов в моде

Если названия отображаются без модов, но исчезают с модом:

1. Откройте `SubModule.xml`
2. Закомментируйте секцию `<Xmls>`:

```xml
  </SubModules>

  <!-- TEMPORARILY DISABLED FOR TESTING
  <Xmls>
    <XmlNode>
      <XmlName id="Settlements" path="settlements"/>
      <IncludedGameTypes>
        <GameType value="Campaign"/>
        <GameType value="CampaignStoryMode"/>
      </IncludedGameTypes>
    </XmlNode>
    <XmlNode>
      <XmlName id="SpriteData" path="SpriteData"/>
      <IncludedGameTypes>
        <GameType value="Campaign"/>
        <GameType value="CampaignStoryMode"/>
      </IncludedGameTypes>
    </XmlNode>
  </Xmls>
  -->
</Module>
```

3. Сохраните файл
4. Запустите игру С модом

**Если названия появились** → Проблема в XML файлах (settlements.xml или SpriteData.xml)
**Если названия всё равно исчезают** → Проблема НЕ в XML, продолжаем диагностику

## Тест 3: Удаление всех UI файлов

Если Тест 2 не помог, попробуем:

1. Временно переименуйте эти файлы (добавьте .bak):
   - `GUI/SpriteData.xml` → `GUI/SpriteData.xml.bak`
   - `Patches/SettlementNameplateViewPatch.cs` → `.bak`
   - `Patches/SettlementNameplatesVMPatch.cs` → `.bak`
   - `ViewModels/CapitalSettlementNameplateVM.cs` → `.bak`

2. Пересоберите проект (Rebuild Solution)

3. Запустите игру

## Тест 4: Только мод без других модов

1. Отключите ВСЕ моды кроме:
   - Bannerlord.Harmony
   - ButterLib
   - MCM (опционально)
   - KingdomCapitals

2. Запустите игру

**Если названия появились** → Конфликт с другим модом!

## Отчёт о результатах

Пожалуйста, проведите все тесты и сообщите результаты:

- Тест 1 (без модов): ✅/❌
- Тест 2 (XML отключены): ✅/❌
- Тест 3 (UI файлы удалены): ✅/❌
- Тест 4 (только мой мод): ✅/❌

А также:
- Список ВСЕХ установленных модов
- Версия игры (точная, например 1.2.12.54620)
- Полные логи мода

---

**ВАЖНО**: settlements.xml НЕ ДОЛЖЕН ломать названия поселений, так как он только добавляет уровни 4-5 для зданий в столицах. Но мы проверим на всякий случай.
