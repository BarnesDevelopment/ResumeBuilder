resource "fusionauth_email" "forgot-password" {
  name                  = "Forgot Password"
  default_from_name     = "Forgot Password"
  default_html_template = file("${path.module}/email_templates/Forgot_Password.html.ftl")
  default_subject       = "Forgot Password"
  default_text_template = file("${path.module}/email_templates/Forgot_Password.txt.ftl")
  from_email            = "example@local.fusionauth.io"
}

resource "fusionauth_email" "setup-password" {
  name                  = "Setup Password"
  default_from_name     = "Setup Password"
  default_html_template = file("${path.module}/email_templates/Setup_Password.html.ftl")
  default_subject       = "Setup Password"
  default_text_template = file("${path.module}/email_templates/Setup_Password.txt.ftl")
  from_email            = "example@local.fusionauth.io"
}
