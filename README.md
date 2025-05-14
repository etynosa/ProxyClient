
#  ProxyClient (.NET 8)

A **RESTful ASP.NET Core Web API** that acts as a proxy for the public API [https://restful-api.dev](https://restful-api.dev), enhancing it with:

- ✅ Filtering, pagination, and caching
- ✅ Full REST support: `GET`, `POST`, `PUT`, `PATCH`, `DELETE`
- ✅ Global exception handling with environment-awareness
- ✅ Docker support

---

## Getting Started

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com) (optional for containerized usage)

---

### Run Locally

```bash
git clone https://github.com/your-username/product-proxy-api.git
cd product-proxy-api
dotnet run
````

Swagger will be available at:

```
https://localhost:{port}/swagger
```

---

###  Run with Docker

```bash
docker build -t product-proxy-api .
docker run -p 5000:80 product-proxy-api
```

Then open:

```
http://localhost:5000/swagger
```

---

## 📁 Project Structure

```
ProductProxyAPI/
├── Controllers/
├── Services/
├── Models/
├── Middleware/
├── Extensions/
├── Program.cs
├── appsettings.json
├── Dockerfile

```

---

## API Endpoints

### `GET /api/v1/products`

Retrieve products with optional filtering and pagination.

**Query Parameters:**

* `name` (optional) — filter by name (case-insensitive)
* `page` (default: 1)
* `pageSize` (default: 10, max: 100)

**Example:**

```bash
curl "https://localhost:5001/api/v1/products?name=phone&page=1&pageSize=5"
```

---

### `GET /api/v1/products/{id}`

Fetch a single product by ID.

```bash
curl https://localhost:5001/api/v1/products/1
```

---

### `POST /api/v1/products`

Create a new product.

**Request Body:**

```json
{
  "name": "New Product",
  "data": {
    "color": "blue",
    "memory": "128GB"
  }
}
```

---

### `PUT /api/v1/products/{id}`

Fully update an existing product.

**Request Body:**

```json
{
  "name": "Updated Product",
  "data": {
    "color": "black",
    "version": "2.0"
  }
}
```

---

### `PATCH /api/v1/products/{id}`

Partially update a product.

**Request Body:**

```json
{
  "name": "Partial Name Update"
}
```

---

### `DELETE /api/v1/products/{id}`

Delete a product by ID.

```bash
curl -X DELETE https://localhost:5001/api/v1/products/1
```

---

##  Exception Handling

All unhandled exceptions return a consistent format:

```json
{
  "success": false,
  "message": "An unexpected error occurred",
  "data": null
}
```

* In **Development**, more error detail is included
* In **Production**, only generic messages are exposed

---

## Validation & Security

* Model validation with `DataAnnotations`
* Central exception middleware with environment-based behavior

---

## Logging

* Logs written to console (custom levels by exception type)
* Includes full stack trace in development
* Structured logs via `ILogger<T>`

