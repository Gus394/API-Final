using System.ComponentModel.DataAnnotations;
namespace Univali.Api.ValidationAttributes;


public class CpfMustBeValidAttribute : ValidationAttribute
{
   public CpfMustBeValidAttribute() { }


   public override bool IsValid(object? value)
   {
       // Verifica se a propriedade Cpf é null
       if (value == null)
       {
           // "true "Se a propriedade Cpf é null não faz nenhuma validação
           // "false "Se a propriedade Cpf é null faz validação
           // Usar true se tiver usando required
           return false;
       }
       // Converte o objeto em uma string
       var cpf = value as string;


       // Verifica se o CPF é válido
       if (!ValidateCPF(cpf!))
       {
           // Retorna que é invalido
           return false;
       }
       // Retorna sucesso
       return true;
   }

   public override string FormatErrorMessage(string name)
   {
       /*
           string.Format() cria uma string formatada combinando um template
               de string com valores fornecidos.
           Primeiro argumento - É uma string que contém o template de string
               com  espaços reservados (marcadores de posição) para os
               valores que serão substituídos.
           Segundo argumento em diante arg0, arg1, ...: São os argumentos
               que serão substituídos nos espaços reservados
           ErrorMessageString possui a mensagem de erro que criamos no Data Annotation
       */
       return string.Format(ErrorMessageString, name);
   }

   // ChatGPT
   private bool ValidateCPF(string cpf)
   {
       // Remove non-numeric characters
       cpf = cpf.Replace(".", "").Replace("-", "");

       // Check if it has 11 digits
       if (cpf.Length != 11)
       {
           return false;
       }

       // Check if all digits are the same
       bool allDigitsEqual = true;
       for (int i = 1; i < cpf.Length; i++)
       {
           if (cpf[i] != cpf[0])
           {
               allDigitsEqual = false;
               break;
           }
       }
       if (allDigitsEqual)
       {
           return false;
       }

       // Check first verification digit
       int sum = 0;
       for (int i = 0; i < 9; i++)
       {
           sum += int.Parse(cpf[i].ToString()) * (10 - i);
       }
       int remainder = sum % 11;
       int verificationDigit1 = remainder < 2 ? 0 : 11 - remainder;
       if (int.Parse(cpf[9].ToString()) != verificationDigit1)
       {
           return false;
       }

       // Check second verification digit
       sum = 0;
       for (int i = 0; i < 10; i++)
       {
           sum += int.Parse(cpf[i].ToString()) * (11 - i);
       }
       remainder = sum % 11;
       int verificationDigit2 = remainder < 2 ? 0 : 11 - remainder;
       if (int.Parse(cpf[10].ToString()) != verificationDigit2)
       {
           return false;
       }

       return true;
   }
}
