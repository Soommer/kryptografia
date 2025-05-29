# Kryptografia — API do szyfrowania i steganografii

Projekt demonstracyjny API umożliwiającego szyfrowanie i deszyfrowanie wiadomości za pomocą różnych algorytmów klasycznych
oraz współczesnych (AES, RSA, SHA-256, Vigenère, Cezar), a także steganografię — ukrywanie wiadomości i obrazów w innych obrazach.

## Funkcjonalności

- Szyfrowanie i deszyfrowanie wiadomości algorytmami:
  - AES (symetryczny, współczesny)
  - RSA (asymetryczny)
  - SHA-256 (funkcja skrótu)
  - Vigenère, Cezar (klasyczne)
- Steganografia:
  - Ukrywanie tekstu w obrazach PNG (LSB)
  - Ukrywanie obrazu w innym obrazie PNG (image-in-image)
- Wybór algorytmu przez API
- Możliwość szyfrowania i deszyfrowania plików przez HTTP

## Architektura

.NET (C#) WebAPI
- Modularna struktura: Controllers, Services, Algorithms, Models, Utils
- Dependency Injection (wzorzec fabryki)
- Asynchroniczne operacje na plikach i tekstach

## Technologie użyte w projekcie

- **Język programowania:**  
  - C# 12, .NET 8.0 (ASP.NET Core WebAPI)
- **Algorytmy kryptograficzne:**  
  - AES, RSA, SHA-256, Vigenère, Cezar
- **Steganografia:**  
  - LSB w obrazach PNG (ImageSharp)
- **API:**  
  - REST, JSON, obsługa plików
- **Dependency Injection:**  
  - .NET Core DI
- **Dokumentacja:**  
  - Swagger / OpenAPI (Swashbuckle)
- **Logowanie:**  
  - Serilog 
- **Konteneryzacja:**  
  - Docker
- **Kontrola wersji:**  
  - Git

## Przykłady użycia API 
	POST /api/encrypt
	{
	  "algorithm": "aes",
	  "message": "Hello world!",
	  "key": "tajnehaslo123"
	}

		Odpowiedź:
		{
			 "encrypted": "string"
		}
	POST /api/decrypt
	{
	  "algorithm": "aes",
	  "message": "base64...",
	  "key": "tajnehaslo123"
	}	
		Odpowiedź:
		{
			"decryptted": "string"
		}

	Swagger API dostępne pod adresem: ../swagger/index.html
