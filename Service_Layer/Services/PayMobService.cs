using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Service_Layer.Services
{
    public class PayMobService : IPayMobService
    {
        private readonly HttpClient _client;

        public PayMobService(HttpClient client)
        {
            _client = client;
        }

        // 1) AUTH → GET TOKEN
        public async Task<string> AuthenticateAsync()
        {
            string apiKey = Environment.GetEnvironmentVariable("PAYMOP_API_KEY") ?? "N/A";

            var body = new { api_key = apiKey };

            var response = await _client.PostAsJsonAsync("auth/tokens", body);
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            return result.token;
        }

        // 2) CREATE ORDER
        public async Task<int> CreateOrderAsync(string token, int amountCents)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new
            {
                amount_cents = amountCents * 100,
                currency = "EGP",
                delivery_needed = false,
                items = Array.Empty<object>()
            };

            var response = await _client.PostAsJsonAsync("ecommerce/orders", body);
            var result = await response.Content.ReadFromJsonAsync<OrderResponse>();

            return result.id;
        }

        // 3) GET PAYMENT KEY
        public async Task<string> GetPaymentKeyAsync(string token, int orderId, int amountCents)
        {
            int integrationId = int.Parse(Environment.GetEnvironmentVariable("PAYMOP_INTEGRATION_ID")!);

            var body = new
            {
                auth_token = token,
                amount_cents = amountCents,
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = "customer@mail.com",
                    floor = "NA",
                    first_name = "John",
                    street = "NA",
                    building = "NA",
                    phone_number = "01000000000",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = "Doe",
                    state = "Cairo"
                },
                currency = "EGP",
                integration_id = integrationId
            };

            var response = await _client.PostAsJsonAsync("acceptance/payment_keys", body);
            var result = await response.Content.ReadFromJsonAsync<PaymentKeyResponse>();

            return result.token;
        }

        // 4) GET IFRAME URL
        public string GetIframeUrl(string paymentKey)
        {
            int iframeId = int.Parse(Environment.GetEnvironmentVariable("PAYMOP_IFRAME_ID")!);
            return $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";
        }

        public async Task<string> PayWithCard(int amountCents)
        {
            var token = await AuthenticateAsync();
            var orderId = await CreateOrderAsync(token, amountCents);
            var paymentKey = await GetPaymentKeyAsync(token, orderId, amountCents);
            return GetIframeUrl(paymentKey);
        }


        public async Task<bool> PaymobCallback([FromBody] PaymobCallback payload, string hmacHeader)
        {
            string secretKey = Environment.GetEnvironmentVariable("HMAC_SECRET_KEY")!; 

            var dataString = $"{payload.obj.amount_cents}" +
                             $"{payload.obj.created_at}" +
                             $"{payload.obj.currency}" +
                             $"{payload.obj.error_occurred}" +
                             $"{payload.obj.has_parent_transaction}" +
                             $"{payload.obj.id}" +
                             $"{payload.obj.integration_id}" +
                             $"{payload.obj.is_3d_secure}" +
                             $"{payload.obj.is_auth}" +
                             $"{payload.obj.is_capture}" +
                             $"{payload.obj.is_refunded}" +
                             $"{payload.obj.is_standalone_payment}" +
                             $"{payload.obj.is_voided}" +
                             $"{payload.obj.order_id}" +
                             $"{payload.obj.owner}" +
                             $"{payload.obj.pending}" +
                             $"{payload.obj.source_data.pan}" +
                             $"{payload.obj.source_data.sub_type}" +
                             $"{payload.obj.source_data.type}" +
                             $"{payload.obj.success}";

            using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataString));
            string calculatedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();

            if (calculatedHmac != hmacHeader)
                return false;

            return payload.obj.success;
        }
    }
    // DTOs
    public class AuthResponse
    {
        public string token { get; set; }
    }

    public class OrderResponse
    {
        public int id { get; set; }
    }

    public class PaymentKeyResponse
    {
        public string token { get; set; }

    }
}
