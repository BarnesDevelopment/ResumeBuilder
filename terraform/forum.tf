resource "fusionauth_tenant" "forum" {
	lifecycle {
		prevent_destroy = true
	}
	issuer   = "forum"
	name     = "Forum"
	theme_id = var.fusionauth_default_theme_id
	multi_factor_configuration {
		login_policy = "Disabled"
	}
	login_configuration {
		require_authentication = false
	}
	external_identifier_configuration {
		authorization_grant_id_time_to_live_in_seconds = 30
		change_password_id_generator {
			length = 32
			type   = "randomBytes"
		}
		change_password_id_time_to_live_in_seconds = 600
		device_code_time_to_live_in_seconds        = 300
		device_user_code_id_generator {
			length = 6
			type   = "randomAlphaNumeric"
		}
		email_verification_id_generator {
			length = 32
			type   = "randomBytes"
		}
		email_verification_id_time_to_live_in_seconds = 86400
		email_verification_one_time_code_generator {
			length = 6
			type   = "randomAlphaNumeric"
		}
		external_authentication_id_time_to_live_in_seconds = 300
		one_time_password_time_to_live_in_seconds          = 60
		passwordless_login_generator {
			length = 32
			type   = "randomBytes"
		}
		passwordless_login_time_to_live_in_seconds = 180
		registration_verification_id_generator {
			length = 32
			type   = "randomBytes"
		}
		registration_verification_id_time_to_live_in_seconds = 86400
		registration_verification_one_time_code_generator {
			length = 6
			type   = "randomAlphaNumeric"
		}
		saml_v2_authn_request_id_ttl_seconds = 300
		setup_password_id_generator {
			length = 32
			type   = "randomBytes"
		}
		setup_password_id_time_to_live_in_seconds = 86400
		two_factor_id_time_to_live_in_seconds     = 300
		two_factor_one_time_code_id_generator {
			length = 6
			type   = "randomDigits"
		}
		two_factor_trust_id_time_to_live_in_seconds = 2592000
	}
	jwt_configuration {
		refresh_token_time_to_live_in_minutes = 43200
		time_to_live_in_seconds               = 3600
	}
	email_configuration {
		host = var.fusionauth_email_configuration_host
		port = var.fusionauth_email_configuration_port
		forgot_password_email_template_id = fusionauth_email.forgot-password.id
		set_password_email_template_id = fusionauth_email.setup-password.id
	}
}

resource "fusionauth_key" "forum-access-token" {
	algorithm = "HS512"
	name      = "Forum Application Access Token Key"
}
