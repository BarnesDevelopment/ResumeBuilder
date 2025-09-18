locals {
  email_templates_path = "${path.module}/email_templates"
  noreply_address     = "noreply@auth.barnes7616.com"
  default_from_name   = "Resume Builder Auth"
}

resource "fusionauth_email" "forgot-password" {
  name                  = "Forgot Password"
  default_from_name     = "Forgot Password"
  default_html_template = file("${path.email_templates_path}/Forgot_Password.html.ftl")
  default_subject       = "Forgot Password"
  default_text_template = file("${local.email_templates_path}/Forgot_Password.txt.ftl")
  from_email            = local.noreply_address
}

resource "fusionauth_email" "setup-password" {
  name                  = "Setup Password"
  default_from_name     = local.default_from_name
  default_html_template = file("${path.email_templates_path}/Setup_Password.html.ftl")
  default_subject       = "Setup Password"
  default_text_template = file("${path.email_templates_path}/Setup_Password.txt.ftl")
  from_email            = local.noreply_address
}
