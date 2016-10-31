using System;
using System.Collections.Generic;
using System.Threading;
using ValidatorExample.ValidationService;

namespace ValidatorExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ServiceSoapClient();
            string[] phonesForValidate = { "+7 495 667 88 99", "9162132632", "8(903)165-34-56", "фываолдж" };

            Console.WriteLine("Пакетная валидация");

            Console.WriteLine("Формирование пакета записей для валидации");
            var batchList = new List<ValidationRequest>();
            for (int i = 0; i < phonesForValidate.Length; i++)
            {
                batchList.Add(new ValidationRequest { RecordId = Guid.NewGuid().ToString(), PhoneNum = phonesForValidate[i]});
            }

            Console.WriteLine("Передача задания на валидацию");
            var jobId = client.ValidateBatch(batchList.ToArray(), false, new Guid("5B2C1A24-60D8-41E5-B0B6-83A7BE51CB81"));

            Console.WriteLine("Проверка состояния задания (выполнено или нет)");
            var status = client.GetJobStatus(jobId, new Guid("5B2C1A24-60D8-41E5-B0B6-83A7BE51CB81"));
            while (status.Status != "DONE")
            {
                
                status = client.GetJobStatus(jobId, new Guid("5B2C1A24-60D8-41E5-B0B6-83A7BE51CB81"));
                Console.WriteLine(String.Format("{0}: Статус задания '{1}'. Обработано телефонных номеров - {2}",
                                                    DateTime.Now, status.Status, status.Count));
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }

            Console.WriteLine("Получение результатов валидации");
            var batchResult = client.GetValidationResult(jobId, new Guid("5B2C1A24-60D8-41E5-B0B6-83A7BE51CB81"));

            for (int i = 0; i < batchResult.Length; i++)
            {
                Console.WriteLine();
                Console.WriteLine(String.Format("Идентификатор записи: {0}", batchResult[i].RecordId));
                Console.WriteLine(String.Format("Статус валидности: {0}", batchResult[i].Status));
                Console.WriteLine(String.Format("Код страны: {0}", batchResult[i].CountryCode));
                Console.WriteLine(String.Format("Телефонный код: {0}", batchResult[i].PhoneCode));
                Console.WriteLine(String.Format("Телефонный номер: {0}", batchResult[i].PhoneNum));
                Console.WriteLine(String.Format("Тип телефона: {0}", batchResult[i].PhoneType));
                Console.WriteLine(String.Format("Оператор связи: {0}", batchResult[i].Provider));

                Console.ReadLine();
            }

        }
    }
}
