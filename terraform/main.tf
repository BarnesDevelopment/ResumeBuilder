terraform {
  required_providers {
    fusionauth = {
      source  = "FusionAuth/fusionauth"
      version = "1.2.0"
    }
# 	portainer = {
# 	  source = "portainer/portainer"
# 	  version = "1.12.2"
# 	}
  }
}

provider "fusionauth" {
  api_key = var.fusionauth_api_key
  host    = var.fusionauth_host
}

# provider "portainer" {
#   endpoint = "https://portainer.example.com"
#   
#     # Option 1: API key authentication
#     api_key  = "your-api-key"
#   
#     # Option 2: Username/password authentication (generates JWT token internally)
#     # api_user     = "admin"
#     # api_password = "your-password"
#   
#     skip_ssl_verify  = true # optional (default value is `false`)
# }